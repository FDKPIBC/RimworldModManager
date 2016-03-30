﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RimworldModOrginiser.DataObjects
{
    class Manager
    {
        public List<ModDetails> ModList
        {
            get
            {
                return this.m_ModList;
            }
            set
            {
                this.m_ModList = value;
            }
        }
        private List<ModDetails> m_ModList = new List<ModDetails>();

        public void LoadModList(string rwFolder)
        {
            this.m_ModList.Clear();

            // Check that the Rimworld Folder exists
            if (!System.IO.Directory.Exists(rwFolder))
            {
                MessageBox.Show("Rimworld Folder not Found" + rwFolder);
                return;
            }

            string _RimworldModsFolder = rwFolder + @"\Mods";
            //Check mods folder exists
            if (!System.IO.Directory.Exists(_RimworldModsFolder))
            {
                MessageBox.Show("Mods Folder not Found" + _RimworldModsFolder);
                return;
            }

            //Find all mods (folders in mods folder)
            System.IO.DirectoryInfo _RimworldModsFolderInfo = new System.IO.DirectoryInfo(_RimworldModsFolder);

            System.IO.DirectoryInfo[] _ModFolders = _RimworldModsFolderInfo.GetDirectories();

            foreach (System.IO.DirectoryInfo _ModDirectoryInfo in _ModFolders)
            {
                RimworldModOrginiser.DataObjects.ModDetails _CurrentMod = new RimworldModOrginiser.DataObjects.ModDetails();
                _CurrentMod.load(_ModDirectoryInfo.FullName);

                this.m_ModList.Add(_CurrentMod);
            }
        }

        public void LoadModConfig(string saveFolder)
        {

            // Check that the Save Folder exists
            if (!System.IO.Directory.Exists(saveFolder))
            {
                MessageBox.Show("Save Folder not Found: " + saveFolder);
                return;
            }

            string _RimworldConfigFolder = saveFolder + @"\Config";
            //Check mods folder exists
            if (!System.IO.Directory.Exists(_RimworldConfigFolder))
            {
                MessageBox.Show("Config Folder not Found:" + _RimworldConfigFolder);
                return;
            }

            //Check File
            string _ModsConfigFilePath = _RimworldConfigFolder + @"\ModsConfig.xml";
            if (!System.IO.File.Exists(_ModsConfigFilePath))
            {
                MessageBox.Show("Mod Config File not Found:" + _ModsConfigFilePath);
                return;
            }

            XmlDocument _XmlFileConfigFile = new XmlDocument();
            _XmlFileConfigFile.Load(_ModsConfigFilePath);

            var _XmlModsConfigData = _XmlFileConfigFile.DocumentElement.SelectSingleNode("/ModsConfigData");

            var _XmlBuildNumber = _XmlFileConfigFile.DocumentElement.SelectSingleNode("buildNumber");

            var _XmlactiveMods = _XmlFileConfigFile.DocumentElement.SelectSingleNode("activeMods");

            List<string> _ActiveMods = new List<string>();

            int i = 0;
            foreach (XmlNode _XmlactiveMod in _XmlactiveMods.ChildNodes)
            {

                _ActiveMods.Add(_XmlactiveMod.InnerText);
                ModDetails _currentMod = this.GetModByName(_XmlactiveMod.InnerText);

                if (_currentMod != null)
                {
                    _currentMod.configValues(i);
                }
                else
                {
                    //Add missing mods

                    ModDetails _NewMod = new ModDetails();

                    _NewMod.Name = _XmlactiveMod.InnerText;
                    _NewMod.configValues(i);

                    this.ModList.Add(_NewMod);

                }

                i++;
            }
        }

        public ModDetails GetModByName(string requiredModName)
        {
            return this.ModList.FirstOrDefault<ModDetails>(m => m.Name == requiredModName);
        }

        public ModDetails GetModBySequence(int requiredSequence)
        {
            return this.ModList.FirstOrDefault<ModDetails>(m => m.Sequence == requiredSequence);
        }

        public void MoveModUp(ModDetails modToMove)
        {
            if (modToMove.Sequence > 0)
            {
                ModDetails _SwappingMod = this.GetModBySequence(modToMove.Sequence - 1);

                modToMove.Sequence -= 1;

                if (_SwappingMod != null)
                {
                    _SwappingMod.Sequence += 1;
                }

            }
        }

        public void MoveModDown(ModDetails modToMove)
        {
            if (modToMove.Sequence > 0)
            {
                ModDetails _SwappingMod = this.GetModBySequence(modToMove.Sequence + 1);

                modToMove.Sequence += 1;

                if (_SwappingMod != null)
                {
                    _SwappingMod.Sequence -= 1;
                }

            }
        }


    }
}