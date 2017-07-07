﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using SharpSvn;
using ViewpointSystems.Svn.SvnThings;


namespace ViewpointSystems.Svn.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private SvnManagement svnManagement;
        private StatusViewModel statusViewModel;

        public MainViewModel()
        {
            Mvx.Trace("Application Start");
        }

        public override void Start()
        {
            base.Start();
        }

        private string repository;
        public string Repository
        {
            get { return repository; }
            set { repository = value; RaisePropertyChanged(() => Repository); }
        }
        private string localWorkingLocation = @"C:\WorkingRepo\";
        public string LocalWorkingLocation
        {
            get { return localWorkingLocation; }
            set { localWorkingLocation = value; RaisePropertyChanged(() => LocalWorkingLocation); }
        }


        private string report;
        public string Report
        {
            get { return report; }
            set { report = value; RaisePropertyChanged(() => Report); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(() => Message); }
        }

        private MvxCommand addCommand;
        public MvxCommand AddCommand
        {
            get
            {
                addCommand = addCommand ?? new MvxCommand(DoAddCommand);
                return addCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Add a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoAddCommand()
        {
            bool addedNotifier = true;
            bool directoryAdded = false;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = svnManagement.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added)
                                {

                                    directoryAdded = true;
                                }
                                else
                                {
                                    svnManagement.Add(directory);
                                    directoryAdded = true;
                                }
                            }
                            else
                            {
                                directoryAdded = true;
                            }
                        }
                    }

                    if (directoryAdded)
                    {
                        foreach (var file in arrAllFiles)
                        {
                            foreach (var subItem in mappings)
                            {
                                if (file == subItem.Key)
                                {
                                    if (subItem.Value.Status.LocalNodeStatus == SvnStatus.NotVersioned)
                                    {
                                        svnManagement.Add(file);
                                        ReportOut("Files added!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private MvxCommand lockCommand;
        public MvxCommand LockCommand
        {
            get
            {
                lockCommand = lockCommand ?? new MvxCommand(DoLockCommand);
                return lockCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Lock on a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoLockCommand()
        {
            bool normalNotifier = true;
            bool directoryNormal = true;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = svnManagement.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                directoryNormal = false;
                            }
                            else
                            {
                                directoryNormal = true;
                            }
                        }
                    }
                }

                if (directoryNormal)
                {
                    foreach (var item in arrAllFiles)
                    {
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Normal)
                                {
                                    normalNotifier = true;
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        svnManagement.SvnLock(item, Message);
                                    }
                                    else
                                    {
                                        ReportOut("Please add a lock message");
                                        return;
                                    }
                                }
                                else
                                {
                                    normalNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    normalNotifier = false;
                }

            }
            if (directoryNormal)
            {
                if (normalNotifier)
                {
                    ReportOut("Locked file successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): Lock file Failed, please make the file is committed.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Lock file Failed, please make sure the directory is committed.");
                return;
            }
        }

        private MvxCommand unlockCommand;
        public MvxCommand UnLockCommand
        {
            get
            {
                unlockCommand = unlockCommand ?? new MvxCommand(DoUnLockCommand);
                return unlockCommand;
            }

        }

        /// <summary>
        /// Performs an Svn UnLock on a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoUnLockCommand()
        {
            bool normalNotifier = true;
            bool directoryNormal = true;
            bool itemLocked = false;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = svnManagement.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                directoryNormal = false;
                            }
                            else
                            {
                                directoryNormal = true;
                            }
                        }
                    }
                }

                if (directoryNormal)
                {
                    foreach (var item in arrAllFiles)
                    {
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Normal)
                                {
                                    normalNotifier = true;
                                    if (subItem.Value.Status.IsLockedLocal)
                                    {
                                        itemLocked = true;
                                        svnManagement.SvnUnLock(item);
                                    }
                                    else
                                    {
                                        itemLocked = false;
                                        ReportOut("Selected item is not Locked!");
                                        return;
                                    }
                                }
                                else
                                {
                                    normalNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    normalNotifier = false;
                }

            }
            if (directoryNormal)
            {
                if (normalNotifier && itemLocked)
                {
                    ReportOut("Unlocked file successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): UnLock file Failed, please make the file is committed and is locked.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Unlock file Failed, please make sure the directory is committed.");
                return;
            }
        }


        private MvxCommand commitCommand;
        public MvxCommand CommitCommand
        {
            get
            {
                commitCommand = commitCommand ?? new MvxCommand(DoCommitCommand);
                return commitCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Commit a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoCommitCommand()
        {
            bool addedNotifier = true;
            bool directoryAdded = true;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = svnManagement.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added)
                                {
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        svnManagement.CommitChosenFiles(directory, Message);
                                        directoryAdded = true;
                                    }
                                    else
                                    {
                                        ReportOut("Please add a commit message");
                                        return;
                                    }
                                    
                                }
                                else
                                {
                                    directoryAdded = false;
                                }
                            }
                            else
                            {
                                directoryAdded = true;
                            }
                        }
                    }
                }

                if (directoryAdded)
                {
                    foreach (var item in arrAllFiles)
                    {
                        string directory = System.IO.Path.GetDirectoryName(item);
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added)
                                {
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        svnManagement.CommitChosenFiles(item, Message);
                                    }
                                    else
                                    {
                                        ReportOut("Please add a commit message");
                                        return;
                                    }
                                }
                                else
                                {
                                    addedNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    addedNotifier = false;
                }
                
            }
            if (directoryAdded)
            {
                if (addedNotifier)
                {
                    ReportOut("Committed all file(s) successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): Commit all file(s) Failed, please make sure all selected files/directories are added.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Commit all file(s) Failed, please make sure the directory is committed.");
                return;
            }
            
        }

        private MvxCommand checkOutCommand;
        public MvxCommand CheckOutCommand
        {
            get
            {
                checkOutCommand = checkOutCommand ?? new MvxCommand(DoCheckOutCommand);
                return checkOutCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Checkout to the specified local directory, which is then your working copy.
        /// </summary>
        private void DoCheckOutCommand()
        {
            if (svnManagement.CheckOut(LocalWorkingLocation))
            {
                ReportOut("Successful Checkout");
                svnManagement.LoadCurrentSvnItemsInLocalRepository(LocalWorkingLocation);
            }
            else
            {
                ReportOut("Checkout Failed");
            }
        }

        private MvxCommand showStatusViewCommand;
        public MvxCommand ShowStatusViewCommand
        {
            get
            {
                showStatusViewCommand = showStatusViewCommand ?? new MvxCommand(DoShowStatusViewCommand);
                return showStatusViewCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Checkout to the specified local directory, which is then your working copy.
        /// </summary>
        private void DoShowStatusViewCommand()
        {
            ShowViewModel<StatusViewModel>(statusViewModel);
        }
        

        /// <summary>
        /// Report messages to the main UI message box.
        /// </summary>
        /// <param name="reportContent"></param>
        private void ReportOut(string reportContent)
        {
            Report = reportContent;
        }

        /// <summary>
        /// On startup, we try and load the svnItems and Status cache from the default directory
        /// </summary>
        public void Loaded()
        {
            svnManagement = Mvx.Resolve<SvnManagement>();
            if (Directory.Exists(LocalWorkingLocation))
            {
                if (svnManagement.IsWorkingCopy(LocalWorkingLocation))
                {
                    svnManagement.LoadCurrentSvnItemsInLocalRepository(LocalWorkingLocation);
                    ReportOut("Loaded current working copy in directory " + LocalWorkingLocation);
                }
                else
                {
                    ReportOut("Directory exists but may not be mapped, try checking out");
                }
            }
            else
            {
                ReportOut("Directory does not exist, try checking out");
            }
        }
    }
}
