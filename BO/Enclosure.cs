﻿using System;
using System.Drawing;

namespace TaskLeader.BO
{
    public abstract class Enclosure
    {
        // Titre de la pièce jointe
        private String v_titre = "";
        public String Titre { get { return v_titre; } set { v_titre = value; } }

        // Icône de la pièce jointe
        private Bitmap v_icone;
        public Image Icone { get { return v_icone; } }

        // Type de la pièce jointe
        private String type_string = "";
        public String TypeSQL { get { return "'" + type_string + "'"; } }

        // Constructeur
        public Enclosure(String titre, String type)
        {
            type_string = type;
            v_titre = titre;

            switch (type)
            {
                case "Mails":
                    v_icone = TaskLeader.Properties.Resources.outlook;
                    break;
				case "Links":
					v_icone = TaskLeader.Properties.Resources.shortcut;
            }          
        }

        // Méthode obligatoire permettant d'ouvrir le lien
        public abstract void open();

        // Méthode obligatoire permettant de stocker le lien
        // Retourne l'ID de stockage
        public abstract String store();
    }

    // Classe générique implémentant la classe abstraite Enclosure
    public class genericEnc : Enclosure
    {
        public genericEnc(String type) : base("", type) { }

        public override void open()
        {
            throw new NotImplementedException();
        }

        public override string store()
        {
            throw new NotImplementedException();
        }

    }
}
