using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DerpScrapper.UI.DialogPanels
{
    public partial class Library : UserControl
    {

        public string errorMessage = "";

        private bool exists = false;
        private DBO.Library library;

        public Library()
        {
            InitializeComponent();

            this.library = new DBO.Library();
            this.loadLanguages();
        }

        public Library(DBO.Library lib)
        {
            InitializeComponent();

            this.library = lib;
            this.exists = true;
            libraryName.Text = this.library.Name;

            this.loadLanguages(lib);
        }

        public bool saveAllowed()
        {
            if (libraryName.Text.ToString() == string.Empty)
            {
                this.errorMessage = "Enter a library name.";
                return false;
            }                

            /* Set the new name to the library DBO and start the naming comparison */
            string oldName = this.library.Name;
            this.library.Name = libraryName.Text.ToString();
            if (this.library.LibraryNameExists())
            {
                this.errorMessage = "The selected library name already exists.";
                this.library.Name = oldName;
                return false;                         
            }

            return true;
        }

        public void storeObject()
        {
            library.Name = this.libraryName.Text.ToString();
            library.PrimaryLanguage = (this.libraryPrimaryLanguage.SelectedItem as Extensions.ComboboxItem).Value.ToString();
            library.SecondaryLanguage = (this.librarySecondaryLanguage.SelectedItem as Extensions.ComboboxItem).Value.ToString();

            if (!this.exists)
                this.library.Insert();
            else
                this.library.Update();
        }

        private void loadLanguages()
        {
            foreach (KeyValuePair<string, string> language in Extensions.Languages())
            {
                Extensions.ComboboxItem item = new Extensions.ComboboxItem();
                item.Text = language.Value.ToString();
                item.Value = language.Key.ToString();

                libraryPrimaryLanguage.Items.Add(item);
                librarySecondaryLanguage.Items.Add(item);
            }

            libraryPrimaryLanguage.SelectedIndex = 0;
            librarySecondaryLanguage.SelectedIndex = 1;
        }

        private void loadLanguages(DBO.Library lib)
        {
            int currentIndex = 0;
            int primarySelectionIndex = 0;
            int secondarySelectionIndex = 1;

            foreach (KeyValuePair<string, string> language in Extensions.Languages())
            {
                Extensions.ComboboxItem item = new Extensions.ComboboxItem();
                item.Text = language.Value.ToString();
                item.Value = language.Key.ToString();

                if (language.Key.ToString() == lib.PrimaryLanguage.ToString())
                    primarySelectionIndex = currentIndex;

                if (language.Key.ToString() == lib.SecondaryLanguage.ToString())
                    secondarySelectionIndex = currentIndex;

                libraryPrimaryLanguage.Items.Add(item);
                librarySecondaryLanguage.Items.Add(item);

                currentIndex++;
            }

            libraryPrimaryLanguage.SelectedIndex = primarySelectionIndex;
            librarySecondaryLanguage.SelectedIndex = secondarySelectionIndex;
        }
    }
}
