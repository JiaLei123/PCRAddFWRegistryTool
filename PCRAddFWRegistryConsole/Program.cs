using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.AccessControl;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace PCRAddFWRegistryConsole
{
    class Program
    {

        #region Fields
        static string cpsFolder = "";
        static string updateFolder;
        static RegistryKey localMachinekey;
        static RegistryKey cpsKey;
        static RegistryKey packKey;
        static List<FWViewModel> LstFWVMs;
        static List<FWViewModel> FilteredLstFWVMs;
        static Regex regex = new Regex(@"^[A-Z]\d{6,8}_\d{6}$");
        #endregion // Fields

        static void Main(string[] args)
        {
            if (GetRegistry())
            {
                SearchAllFWs();
                Create();
            }
            Console.ReadLine();
        }


        static bool GetRegistry()
        {
            try
            {
                Console.WriteLine("------查找CPS安装目录------");

                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(userName,
                    RegistryRights.FullControl,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));

                RegistryKey localMachinekey = Registry.LocalMachine;

                cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Motorola\MOTOTRBO CPS", true);

                if (cpsKey == null)
                {
                    Console.WriteLine("PCR CPS 没安装!");
                    return false;
                }

                cpsFolder = (string)cpsKey.GetValue("DefaultFolderPath");

                if (string.IsNullOrWhiteSpace(cpsFolder))
                {
                    Console.WriteLine("PCR CPS 没安装!");
                    return false;
                }
                else
                {
                    updateFolder = Path.Combine(cpsFolder, @"deviceupdate\update");
                    if (!Directory.Exists(updateFolder))
                        Directory.CreateDirectory(updateFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("PCR CPS 没安装，或没以管理员身份运行程序!");

                return false;
            }
            return true;
        }

        static List<string> lstVersionFolers = null;

        static void GetVerFolders(string folder)
        {

            if (regex.IsMatch(new DirectoryInfo(folder).Name))
            {
                lstVersionFolers.Add(folder);
            }


            foreach (string dir in Directory.GetDirectories(folder))
            {
                GetVerFolders(dir);
            }
        }


        static void SearchAllFWs()
        {
            Console.WriteLine("------搜索所有安装目录下的FW包------");
            LstFWVMs = new List<FWViewModel>();
            lstVersionFolers = new List<string>();
            GetVerFolders(updateFolder);
            foreach (string verFolder in lstVersionFolers)
            {
                string pak32 = Directory.GetDirectories(verFolder)[0];
                foreach (string pm in Directory.GetDirectories(pak32))
                {
                    foreach (string modelFolder in Directory.GetDirectories(pm))
                    {
                        FWViewModel fwVM = new FWViewModel(pm, modelFolder);
                        LstFWVMs.Add(fwVM);
                    }
                }
            }

        }


        static Dictionary<string, string> dicRegion = new Dictionary<string, string>
        {
            {"N","NA"},
            {"A","AS"},
            {"E","EMEA"},
            {"L","LA"}
        };

        static void CreateOneVersion(IEnumerable<FWViewModel> lstFWVMs)
        {
            if (lstFWVMs == null || lstFWVMs.Count() == 0)
                return;

            string now = DateTime.Now.ToString("_yyyyMMddHHmmssfff");
            FWViewModel firstFWVM = lstFWVMs.First();

            string strFWKey = firstFWVM.Version + now;

            RegistryKey fwKey = packKey.CreateSubKey(strFWKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

            if (fwKey == null)
            {
                Console.WriteLine(string.Format("出错，不能创建节点 {0}！", strFWKey));
                return;
            }


            int cnt = 0;
            foreach (FWViewModel fwVMs in lstFWVMs)
            {
                cnt++;
                fwKey.SetValue(string.Format("Path{0}", cnt), fwVMs.FolderName2PM);
            }

            fwKey.SetValue("Region", dicRegion[firstFWVM.Region]);
            fwKey.SetValue("Title", "Title" + strFWKey);
            fwKey.SetValue("Type", "Type" + strFWKey);
            fwKey.Close();
        }

        static void Create()
        {
            try
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                RegistrySecurity rs = new RegistrySecurity();
                rs.AddAccessRule(new RegistryAccessRule(userName,
                    RegistryRights.FullControl,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));

                localMachinekey = Registry.LocalMachine;
                string strPackages = "Packages";
                packKey = cpsKey.OpenSubKey(strPackages, true);
                if (packKey == null)
                {
                    cpsKey.CreateSubKey(strPackages, RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                packKey = cpsKey.OpenSubKey(strPackages, true);
                if (packKey == null)
                {
                    Console.WriteLine("出错： 不能创建'Packages'注册表节点");
                    return;
                }






                FilteredLstFWVMs = LstFWVMs.Where(x => x.Region == "A").ToList();

                if (FilteredLstFWVMs.Count > 0)
                {
                    Console.WriteLine("------开始为以下文件(AS 地区)添加注册表------");

                    foreach (var fw in FilteredLstFWVMs)
                    {
                        Console.WriteLine(string.Format("{0}", fw.FolderToolTip));
                    }

                    //DeleteAllFirst
                    Console.WriteLine("------删除原有相关注册表------");
                    foreach (var v in packKey.GetSubKeyNames())
                    {
                        packKey.DeleteSubKey(v);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("------未在{0}找到可用的FW(AS 地区)，未添加任何注册表------",updateFolder));
                    return;
                }



                var lstVersions = FilteredLstFWVMs.Select(x => x.Version).Distinct();
                foreach (string ver in lstVersions)
                {
                    var lstp = FilteredLstFWVMs.Where(x => x.Version == ver && new DirectoryInfo(x.FolderName2PM).Name.Contains("portable"));//portable

                    var lstm = FilteredLstFWVMs.Where(x => x.Version == ver && new DirectoryInfo(x.FolderName2PM).Name.Contains("mobile"));

                    CreateOneVersion(lstp);
                    CreateOneVersion(lstm);

                }

                packKey.Close();

                Console.WriteLine("------完成------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
