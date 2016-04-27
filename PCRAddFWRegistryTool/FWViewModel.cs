using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.IO;
namespace PCRAddFWRegistryTool
{
  public  class FWViewModel:ViewModelBase
    {

        #region Fields
        #endregion // Fields

        #region Constructor
      public FWViewModel(string pmf,string modelf)
      {
          this.FolderName2PM = pmf;
          this.FolderName2Model = modelf;

          string lastFolder = new DirectoryInfo(this.FolderName2Model).Name;
          string[] ss = lastFolder.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
          if (lastFolder.Contains("T3000A"))
          {
              this.Region = ss[1];
              this.Version = ss[2] + "_" + ss[3];
              this.Model = ss[0];
          }
          else
          {
              this.Region = ss[2];
              this.Version = ss[3] + "_" + ss[4];
              this.Model = ss[0];
          }

      }
        #endregion // Constructor

        #region Properties



      private string region;
      public string Region
      {
          get { return region; }
          set
          {
              if (region != value)
              {
                  region = value;
                  base.RaisePropertyChanged("Region");
              }
          }
      }


        //private string title;
        //public string Title
        //{
        //    get { return title; }
        //    set
        //    {
        //        if (title != value)
        //        {
        //            title = value;
        //            base.RaisePropertyChanged("Title");
        //        }
        //    }
        //}


        //private string type;
        //public string Type
        //{
        //    get { return type; }
        //    set
        //    {
        //        if (type != value)
        //        {
        //            type = value;
        //            base.RaisePropertyChanged("Type");
        //        }
        //    }
        //}


        private string folderToolTip;
        public string FolderToolTip
        {
            get 
            {
                //return string.Format(@"{0}\r\n{1}",this.FolderName2PM,this.FolderName2Model);
                string part = @"MOTOTRBO CPS\deviceupdate\update";
                int index=this.FolderName2PM.IndexOf(part);

                return string.Format(@"{0}", this.FolderName2PM.Substring(index + part.Length, this.FolderName2PM.Length - index - part.Length));

            }
           
        }
        

        private string folderName2PM;
        public string FolderName2PM
        {
            get { return folderName2PM; }
            set
            {
                if (folderName2PM != value)
                {
                    folderName2PM = value;
                    base.RaisePropertyChanged("FolderName2PM");
                }
            }
        }


        private string folderName2Model;
        public string FolderName2Model
        {
            get { return folderName2Model; }
            set
            {
                if (folderName2Model != value)
                {
                    folderName2Model = value;
                    base.RaisePropertyChanged("FolderName2Model");
                }
            }
        }
        
        

        private string model;
        public string Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    base.RaisePropertyChanged("Model");
                }
            }
        }
        

        private string version;
        public string Version
        {
            get { return version; }
            set
            {
                if (version != value)
                {
                    version = value;
                    base.RaisePropertyChanged("Version");
                }
            }
        }



        //private bool isChecked;
        //public bool IsChecked
        //{
        //    get { return isChecked; }
        //    set
        //    {
        //        if (isChecked != value)
        //        {
        //            isChecked = value;
        //            base.RaisePropertyChanged("IsChecked");
        //        }
        //    }
        //}
        
        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Public Methods
        #endregion // Public Methods

        #region Private Helpers
       

        #endregion // Private Helpers

    }
}
