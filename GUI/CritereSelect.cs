﻿using System;
using System.Collections;
using System.Windows.Forms;
using TaskLeader.BO;
using TaskLeader.DAL;

namespace TaskLeader.GUI
{
    public partial class CritereSelect : UserControl
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur pour le Designer
        /// </summary>
        public CritereSelect()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructeur pour un Criterium
        /// </summary>
        /// <param name="title">Titre du critère (et aussi nom du contrôle)</param>
        public CritereSelect(DBentity entity)
        {
            InitializeComponent();
            this.titre.Text = entity.nom;
            this.type = entity;
        }

        #endregion

        #region Gestion des dépendances entre CritereSelect

        private bool hasParent = false;

        /// <summary>
        /// Evènement déclenché lors du changement de sélection dans la liste du MultipleSelect
        /// </summary>
        public event EventHandler SelectedIndexChanged
        {
            add { this.liste.SelectedIndexChanged += value; }
            remove { this.liste.SelectedIndexChanged -= value; }
        }

        /// <summary>
        /// Rend dépendant ce widget d'un autre
        /// </summary>
        /// <param name="widget">MultipleSelect parent</param>
        public void addParent(CritereSelect widget)
        {
            this.hasParent = true;
            widget.SelectedIndexChanged += new EventHandler(this.liste_SelectedIndexChanged);
        }

        /// <summary>
        /// Mis à jour du widget en fonction de l'état du parent
        /// </summary>
        private void liste_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(this.type.nom + " doit changer");

            CheckedListBox criteres = ((CheckedListBox)sender);
            CheckedListBox.CheckedItemCollection items = criteres.CheckedItems;
            DB db = (DB)criteres.Tag;

            // On n'affiche la liste des sujets que si un seul contexte est tické
            if (items.Count == 1)
            {
                this.maj(items[0].ToString());
                this.box.Enabled = true;
            }
            else
            {
                this.liste.Items.Clear();
                this.box.Checked = true;
                this.box.Enabled = false;
            }
        }

        #endregion

        #region Méthodes internes au widgets

        /// <summary>
        /// Méthode appelée si checkbox 'Tous' sélectionnée
        /// </summary>
        private void box_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.liste.Items.Count; i++)
                this.liste.SetItemChecked(i, this.box.Checked);
        }

        /// <summary>
        /// Méthode appelée si click sur la liste
        /// </summary>
        private void liste_Click(object sender, EventArgs e)
        {
            if (box.Checked)
                box.Checked = false;
        }

        #endregion

        #region Membres propres à un Criterium
        /// <summary>
        /// Un CritereSelect peut se rafraîchir sur les triggers suivants:
        /// - Changement de base de référence (sauf pour les widgets enfants)
        /// - Nouvelle valeur pour la base courante
        /// </summary>

        private DBentity type;
        // DB attachée à la CheckedListBox pour être récupérée avec les EventArgs
        private DB db { get { return (DB)this.liste.Tag; } set { this.liste.Tag = value; } }

        /// <summary>
        /// Applique un critère au MultipleSelect.
        /// Pas utilisé pour le moment mais le sera si édition d'un filtre
        /// </summary>
        /// <param name="critere">Criterium à appliquer</param>
        private void apply(Criterium critere)
        {
            box.Checked = false; // La checkbox "Tous" n'est pas sélectionnée
            for (int i = 0; i < liste.Items.Count; i++) // Parcours de la ListBox
            {
                int index = critere.selected.IndexOf(liste.Items[i]); // Recherche de l'item dans le filtre
                liste.SetItemChecked(i, !(index == -1));
            }
        }

        /// <summary>
        /// Renvoie le Criterium correspondant ou null
        /// </summary>
        public Criterium getCriterium()
        {
            if (!box.Checked)
                return new Criterium(type, liste.CheckedItems);
            else
                return null;
        }

        /// <summary>
        /// Changement de la DB de référence
        /// </summary>
        /// <param name="db">Nouvelle DB</param>
        public void changeDB(DB database)
        {
            if (!this.hasParent) // Les contrôles enfants ne doivent pas être mis à jour directement
            {
                // Unregister de l'ancienne DB
                if(this.db != null)
                    this.db.unsubscribe_NewValue(this.type, new EventHandler(newValue));
                // Mémorisation de la "nouvelle" DB
                this.db = database;
                // Register de la nouvelle DB
                this.db.subscribe_NewValue(this.type, new EventHandler(newValue));
                this.maj();
            }
        }

        private void maj(String key=null)
        {
            this.liste.ClearSelected(); // Permet de déclencher l'évènement SelectedIndexChanged
            this.liste.Items.Clear(); // Vidage de la liste
            foreach (object item in this.db.getTitres(this.type, key))
                this.liste.Items.Add(item, true); // Sélection de toutes les valeurs
            this.box.Checked = true;
        }

        private void newValue(object sender, EventArgs e)
        {
            this.maj("");
            //TODO: si Parent, this.maj()
            // si Enfant et widget actif, this.maj(key)
        }

        #endregion

        #region Membres propres à une liste de DB

        /// <summary>
        /// Modifie le label du widget pour une liste de DB
        /// </summary>
        public void setForDB()
        {
            this.titre.Text = "Base d'actions";
        }

        /// <summary>
        /// Ajoute une DB à la liste du widget
        /// </summary>
        /// <param name="db">La DB à ajouter</param>
        public void addDB(DB db)
        {
            this.liste.Items.Add(db,true);
        }

        /// <summary>
        /// Supprimer une DB de la liste de widget
        /// </summary>
        /// <param name="db">La DB à supprimer</param>
        public void removeDB(DB db)
        {
            this.liste.Items.Remove(db);
        }

        public object[] getDBs()
        {
            return new ArrayList(this.liste.CheckedItems).ToArray();
        }
        
        #endregion
    }
}
