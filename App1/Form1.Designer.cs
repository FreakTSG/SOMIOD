﻿namespace App1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PortaoImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PortaoImage)).BeginInit();
            this.SuspendLayout();
            // 
            // PortaoImage
            // 
            this.PortaoImage.Image = global::App1.Properties.Resources.fechado;
            this.PortaoImage.Location = new System.Drawing.Point(21, 23);
            this.PortaoImage.Name = "PortaoImage";
            this.PortaoImage.Size = new System.Drawing.Size(437, 391);
            this.PortaoImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PortaoImage.TabIndex = 0;
            this.PortaoImage.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 450);
            this.Controls.Add(this.PortaoImage);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1Initialize);
            ((System.ComponentModel.ISupportInitialize)(this.PortaoImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PortaoImage;
    }
}

