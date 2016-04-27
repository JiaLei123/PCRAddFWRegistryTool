using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace PCRAddFWRegistryConsole
{
  public  class FWViewModel
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
          this.Region = ss[2];
          this.Version =ss[3] + "_" + ss[4] ;
          this.Model = ss[0];
      }
        #endregion // Constructor

        #region Properties



     
      public string Region
      {
          get; 
          set;
      }



        
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
        


        public string FolderName2PM
        {
            get;
            set;
        }



        public string FolderName2Model
        {
            get;
            set;
        }
        
        

        public string Model
        {
            get;
            set;
        }
        

        public string Version
        {
            get;
            set;
        }



     
        
        #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Public Methods
        #endregion // Public Methods

        #region Private Helpers
       

        #endregion // Private Helpers

    }
}
