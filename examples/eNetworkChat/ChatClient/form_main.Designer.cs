namespace ChatClient
{
    partial class form_main
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.listBox_messages = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // textBox_message
            // 
            this.textBox_message.Location = new System.Drawing.Point(12, 430);
            this.textBox_message.MaxLength = 256;
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.Size = new System.Drawing.Size(360, 20);
            this.textBox_message.TabIndex = 0;
            this.textBox_message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_message_KeyDown);
            // 
            // listBox_messages
            // 
            this.listBox_messages.FormattingEnabled = true;
            this.listBox_messages.Location = new System.Drawing.Point(12, 12);
            this.listBox_messages.Name = "listBox_messages";
            this.listBox_messages.Size = new System.Drawing.Size(360, 407);
            this.listBox_messages.TabIndex = 1;
            // 
            // form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 462);
            this.Controls.Add(this.listBox_messages);
            this.Controls.Add(this.textBox_message);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 500);
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "form_main";
            this.ShowIcon = false;
            this.Text = "ChatClient using eNetwork2";
            this.Load += new System.EventHandler(this.form_main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.ListBox listBox_messages;
    }
}

