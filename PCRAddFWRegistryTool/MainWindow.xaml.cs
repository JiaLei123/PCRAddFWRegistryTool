using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.IO;
using System.Diagnostics;
using System.Reflection;
namespace PCRAddFWRegistryTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow = null;
        public   CollectionView cv = null;
        private string cpsFolder = "";
        private string updateFolder = "";
        private List<string> lstVersionFolders;
        private RegistryKey localMachinekey;
        private RegistryKey cpsKey;
        private RegistryKey packKey;

        public MainViewModel mainVM = null;
        public MainWindow()
        {
            InitializeComponent();
            MainWindow.mainWindow = this;
            this.mainVM = new MainViewModel();
            this.DataContext = this.mainVM;
            //this.FilterByRegion(this.mainVM.SelectedRegion);//
         
        }


        private void cboRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtonState();
        }

        private void SetButtonState()
        {
            //this.btnCreate.IsEnabled = cboRegion.SelectedValue != null
            //&& !String.IsNullOrWhiteSpace((cboRegion.SelectedItem as ComboBoxItem).Content.ToString())
            //&& this.listBox1.Items.Count > 0
            //&& !string.IsNullOrWhiteSpace(this.cpsFolder);

            //this.btnRefresh.IsEnabled = !string.IsNullOrWhiteSpace(this.cpsFolder);

            //this.hyperlink1.IsEnabled = !string.IsNullOrWhiteSpace(this.updateFolder);
        }



        //private void btnCreate_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //        RegistrySecurity rs = new RegistrySecurity();
        //        rs.AddAccessRule(new RegistryAccessRule(userName,
        //            RegistryRights.FullControl,
        //            InheritanceFlags.ObjectInherit,
        //            PropagationFlags.InheritOnly,
        //            AccessControlType.Allow));

        //        localMachinekey = Registry.LocalMachine;
        //        string strPackages = "Packages";
        //        packKey = cpsKey.OpenSubKey(strPackages, true);
        //        if (packKey == null)
        //        {
        //            cpsKey.CreateSubKey(strPackages, RegistryKeyPermissionCheck.ReadWriteSubTree);
        //        }
        //        if (packKey == null)
        //        {
        //            MessageBox.Show("Error：cannot create 'Packages'");
        //            return;
        //        }



        //        if (chkDelete.IsChecked ?? false)
        //        {
        //            foreach (var v in packKey.GetSubKeyNames())
        //            {
        //                packKey.DeleteSubKey(v);
        //            }
        //        }

            
        //        foreach (string version in this.lstVersionFolders)
        //            CreateOneVersion(version);

        //        packKey.Close();

        //        MessageBox.Show("Success!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}


        //private void CreateOneVersion(string versionFullPath)
        //{
        //    string now = DateTime.Now.ToString("_yyyyMMddHHmmss");
        //    string strFWKey = new DirectoryInfo(versionFullPath).Name + now;
        //    RegistryKey fwKey = packKey.CreateSubKey(strFWKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

        //    if (fwKey == null)
        //    {
        //        MessageBox.Show(string.Format("Error：cannot create {0}！", strFWKey));
        //        return;
        //    }

        //    string Pakpd032 = Directory.GetDirectories(versionFullPath)[0];

        //    int cnt = 0;
        //    foreach (string pm in Directory.GetDirectories(Pakpd032))
        //    {
        //        cnt++;
        //        fwKey.SetValue(string.Format("Path{0}", cnt), pm);
        //    }

        //    fwKey.SetValue("Region", (this.cboRegion.SelectedItem as ComboBoxItem).Content.ToString());
        //    fwKey.SetValue("Title", "Title" + strFWKey);
        //    fwKey.SetValue("Type", "Type" + strFWKey);

        //    fwKey.Close();
        //}


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.Title += " Version: " + Assembly.GetExecutingAssembly().GetName().Version;

        //    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //    RegistrySecurity rs = new RegistrySecurity();


        //    rs.AddAccessRule(new RegistryAccessRule(userName,
        //        RegistryRights.FullControl,
        //        InheritanceFlags.ObjectInherit,
        //        PropagationFlags.InheritOnly,
        //        AccessControlType.Allow));

        //    RegistryKey localMachinekey = Registry.LocalMachine;


        //    RegistryKey sow6432NodeKey = localMachinekey.OpenSubKey(@"SOFTWARE\Wow6432Node", true);

        //    if (sow6432NodeKey != null)
        //    {
        //        cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Wow6432Node\Motorola\MOTOTRBO CPS", true);
        //    }
        //    else
        //    {
        //        cpsKey = localMachinekey.OpenSubKey(@"SOFTWARE\Motorola\MOTOTRBO CPS", true);
        //    }

        //    if (cpsKey==null)
        //    {
        //        this.txtUpateFolder.Text = "PCR CPS not installed!";
        //        SetButtonState();
        //    }

        //    cpsFolder = (string)cpsKey.GetValue("DefaultFolderPath");

        //    if (string.IsNullOrWhiteSpace(cpsFolder))
        //    {
        //        this.txtUpateFolder.Text = "PCR CPS not installed!";
        //    }
               
        //    else
        //    {
        //        this.updateFolder = Path.Combine(cpsFolder, @"deviceupdate\update");
        //        this.txtUpateFolder.Text = this.updateFolder;
        //    }
        //    SetButtonState();
        //    RefreshVersions();

        }


        //private void btnRefresh_Click(object sender, RoutedEventArgs e)
        //{
        //    RefreshVersions();
        //}


        //private void RefreshVersions()
        //{
        //    if (!Directory.Exists(this.updateFolder))
        //        Directory.CreateDirectory(this.updateFolder);

        //    Regex regex = new Regex(@"^[A-Z]\d{6}_\d{6}$");

        //    this.listBox1.Items.Clear();
        //    this.lstVersionFolders = Directory.GetDirectories(this.updateFolder)
        //        .Where(x => regex.IsMatch(new DirectoryInfo(x).Name))
        //        .ToList();
        //    foreach (string version in this.lstVersionFolders)
        //    {
        //        this.listBox1.Items.Add(new DirectoryInfo(version).Name);
        //    }
        //    SetButtonState();
        //}


        //private void hyperlink1_Click(object sender, RoutedEventArgs e)
        //{
          
           
        //}

        //private void Window_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    localMachinekey.Close();
        //    cpsKey.Close();
        //}

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            FWViewModel fwVM = e.Item as FWViewModel;
            if(fwVM.Region==this.mainVM.SelectedRegion)
            {
                e.Accepted=true;
            }
            else
            {
                e.Accepted=false;
            }
        }

        //private void cboRegion_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        //{
             
        //    //string re = this.cboRegion.Text.Trim();
        //    string re =e.AddedItems[0] as string;
        //    this.mainVM.SelectedRegion = re;
        //    CollectionView cv = (this.FindResource("cvs") as CollectionViewSource).View as CollectionView ;
        //    cv.Refresh();
        //}

        public void FilterByRegion(string re)
        {
            this.cv = (this.FindResource("cvs") as CollectionViewSource).View as CollectionView;
            if (this.cv != null)
            {
                this.cv.Refresh();
                
            }


            //var v = cv.Cast<FWViewModel>().Count();
            //MessageBox.Show(v.ToString());

        }



    }


}
