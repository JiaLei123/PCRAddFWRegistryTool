using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Security.AccessControl;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace PCRAddFWRegistryTool
{
    public class MainViewModel : ViewModelBase
    {

        #region Fields
        private string cpsFolder = "";
        private RegistryKey localMachinekey;
        private RegistryKey cpsKey;
        private RegistryKey packKey;
        #endregion // Fields

        #region Constructor
        public MainViewModel()
        {
            this.SelectedRegion = "N";
            this.GetRegistry();
            this.SearchAllFWs();

        }
        #endregion // Constructor

        #region Properties



        private ObservableCollection<FWViewModel> lstFWVMs;
        public ObservableCollection<FWViewModel> LstFWVMs
        {
            get { return lstFWVMs; }
            set
            {
                if (lstFWVMs != value)
                {
                    lstFWVMs = value;
                    base.RaisePropertyChanged("LstFWVMs");
                }
            }
        }




        public IEnumerable<FWViewModel> FilteredLstFWVMs
        {
            get
            {
                return MainWindow.mainWindow.cv.Cast<FWViewModel>();
            }

        }


        private string updateFolder;
        public string UpdateFolder
        {
            get { return updateFolder; }
            set
            {
                if (updateFolder != value)
                {
                    updateFolder = value;
                    base.RaisePropertyChanged("UpdateFolder");
                }
            }
        }


        //public IEnumerable<FWViewModel> LstSelectedRegNodeVMs
        //{
        //    get
        //    {
        //        return  this.LstRegNodeVMs.Where(x=>x.IsChecked); 
        //    }

        //}





        private bool deleteAllFirst;
        public bool DeleteAllFirst
        {
            get { return deleteAllFirst; }
            set
            {
                if (deleteAllFirst != value)
                {
                    deleteAllFirst = value;
                    base.RaisePropertyChanged("DeleteAllFirst");
                }
            }
        }


        private string selectedRegion;
        public string SelectedRegion
        {
            get { return selectedRegion; }
            set
            {
                if (selectedRegion != value)
                {
                    selectedRegion = value;

                    base.RaisePropertyChanged("SelectedRegion");
                    MainWindow.mainWindow.FilterByRegion(this.SelectedRegion);
                }
            }
        }


        #endregion // Properties

        #region Commands

        RelayCommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new RelayCommand
                        (this.Refresh, () => this.CanRefresh);
                }
                return refreshCommand;
            }
        }

        bool CanRefresh
        {
            get
            {
                return true;
            }
        }

        public void Refresh()
        {
            this.SearchAllFWs();
        }



        RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get
            {
                if (createCommand == null)
                {
                    createCommand = new RelayCommand
                        (this.Create, () => this.CanCreate);
                }
                return createCommand;
            }
        }

        bool CanCreate
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.SelectedRegion) &&
                    this.LstFWVMs.Count() > 0;
            }
        }

        public void Create()
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
                if (packKey == null)
                {
                    MessageBox.Show("Error：cannot create 'Packages'");
                    return;
                }

                if (this.DeleteAllFirst)
                {
                    foreach (var v in packKey.GetSubKeyNames())
                    {
                        packKey.DeleteSubKey(v);
                    }
                }


                MainWindow.mainWindow.FilterByRegion(this.SelectedRegion);//


                var lstVersions = this.FilteredLstFWVMs.Select(x => x.Version).Distinct();
                foreach (string ver in lstVersions)
                {
                    var lstp = this.FilteredLstFWVMs.Where(x => x.Version == ver && new DirectoryInfo(x.FolderName2PM).Name.Contains("portable"));//portable
                    var lstm = this.FilteredLstFWVMs.Where(x => x.Version == ver && new DirectoryInfo(x.FolderName2PM).Name.Contains("mobile"));
                    var lstre = this.FilteredLstFWVMs.Where(x => x.Version == ver && new DirectoryInfo(x.FolderName2PM).Name.Contains("repeater"));

                    CreateOneVersion(lstp);
                    CreateOneVersion(lstm);
                    CreateOneVersion(lstre);

                }
                packKey.Close();

                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        RelayCommand openUpdateFolderCommand;
        public ICommand OpenUpdateFolderCommand
        {
            get
            {
                if (openUpdateFolderCommand == null)
                {
                    openUpdateFolderCommand = new RelayCommand
                        (this.OpenUpdateFolder, () => this.CanOpenUpdateFolder);
                }
                return openUpdateFolderCommand;
            }
        }

        bool CanOpenUpdateFolder
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UpdateFolder);

            }
        }

        public void OpenUpdateFolder()
        {
            Process.Start("explorer.exe", this.updateFolder);
        }


        #endregion // Commands

        #region Public Methods
        #endregion // Public Methods

        #region Private Helpers

        private void CreateOneVersion(IEnumerable<FWViewModel> lstFWVMs)
        {
            if (lstFWVMs == null || lstFWVMs.Count() == 0)
                return;

            string now = DateTime.Now.ToString("_yyyyMMddHHmmssfff");
            FWViewModel firstFWVM = lstFWVMs.First();
            //string strFWKey = new DirectoryInfo(versionFullPath).Name + now;
            string strFWKey = Guid.NewGuid().ToString();

            RegistryKey fwKey = packKey.CreateSubKey(strFWKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

            if (fwKey == null)
            {
                MessageBox.Show(string.Format("Error：cannot create {0}！", strFWKey));
                return;
            }

            //string Pakpd032 = Directory.GetDirectories(versionFullPath)[0];

            HashSet<string> pmFolders = new HashSet<string>();
            foreach (FWViewModel fwVMs in lstFWVMs)
            {
                pmFolders.Add(fwVMs.FolderName2PM);
            }

            int cnt = 0;
            foreach (string pmFolder in pmFolders)
            {
                cnt++;
                fwKey.SetValue(string.Format("Path{0}", cnt), pmFolder);
            }

            //foreach (string pm in Directory.GetDirectories(Pakpd032))
            //{
            //    cnt++;
            //    fwKey.SetValue(string.Format("Path{0}", cnt), pm);
            //}

            //fwKey.SetValue("Region", (this.cboRegion.SelectedItem as ComboBoxItem).Content.ToString());
            //fwKey.SetValue("Title", "Title" + strFWKey);
            //fwKey.SetValue("Type", "Type" + strFWKey);
            fwKey.SetValue("Region", this.dicRegion[firstFWVM.Region]);
            fwKey.SetValue("Title", "Title" + strFWKey);
            fwKey.SetValue("Type", "Type" + strFWKey);
            fwKey.Close();
        }


        private Dictionary<string, string> dicRegion = new Dictionary<string, string>
        {
            {"N","NA"},
            {"A","AS"},
            {"E","EMEA"},
            {"L","LA"}
        };

        private void GetRegistry()
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

                RegistryKey localMachinekey = Registry.LocalMachine;


                //RegistryKey sow6432NodeKey = localMachinekey.OpenSubKey(@"SOFTWARE\Wow6432Node", true);

                //if (sow6432NodeKey != null)
                //{
                //cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Wow6432Node\Motorola\MOTOTRBO CPS", true);
                //}
                //else
                //{
                //    cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Motorola\MOTOTRBO CPS", true);
                //}

                cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Motorola\MOTOTRBO CPS", true);

                if (cpsKey == null)
                {
                    MessageBox.Show("PCR CPS not installed!");

                }

                cpsFolder = (string)cpsKey.GetValue("DefaultFolderPath");

                if (string.IsNullOrWhiteSpace(cpsFolder))
                {
                    MessageBox.Show("PCR CPS not installed!");


                }

                else
                {
                    this.UpdateFolder = Path.Combine(cpsFolder, @"deviceupdate\update");
                    //
                    if (!Directory.Exists(this.updateFolder))
                        Directory.CreateDirectory(this.updateFolder);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Dictionary<string, string> dicVerModel = null;
        private Regex regex = new Regex(@"^[A-Z]\d{6,8}_\d{6}$");
        private void SearchAllFWs()
        {
            this.LstFWVMs = new ObservableCollection<FWViewModel>();
            //this.dicVerModel = new Dictionary<string, string>();
            this.lstVersionFolers = new List<string>();
            this.GetVerFolders(this.UpdateFolder);
            foreach (string verFolder in this.lstVersionFolers)
            {
                foreach(string pak32 in Directory.GetDirectories(verFolder))
                {
                    foreach (string pm in Directory.GetDirectories(pak32))
                    {
                        foreach (string modelFolder in Directory.GetDirectories(pm))
                        {
                            FWViewModel fwVM = new FWViewModel(pm, modelFolder);

                            this.LstFWVMs.Add(fwVM);
                        }
                    }
                }
                
            }

        }

        private List<string> lstVersionFolers = null;

        private void GetVerFolders(string folder)
        {

            if (this.regex.IsMatch(new DirectoryInfo(folder).Name))
            {
                this.lstVersionFolers.Add(folder);
            }


            foreach (string dir in Directory.GetDirectories(folder))
            {
                GetVerFolders(dir);
            }


        }

        public override void Cleanup()
        {
            try
            {
                this.localMachinekey.Close();
                this.cpsKey.Close();
                base.Cleanup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion // Private Helpers

    }
}
