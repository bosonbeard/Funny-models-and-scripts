namespace Fortuneteller
{
    partial class ListEditorForm
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
            this.Lpredictions = new System.Windows.Forms.ListBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.DelBtn = new System.Windows.Forms.Button();
            this.AdBtn = new System.Windows.Forms.Button();
            this.SaceBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Lpredictions
            // 
            this.Lpredictions.FormattingEnabled = true;
            this.Lpredictions.ItemHeight = 16;
            this.Lpredictions.Location = new System.Drawing.Point(17, 21);
            this.Lpredictions.Name = "Lpredictions";
            this.Lpredictions.Size = new System.Drawing.Size(353, 148);
            this.Lpredictions.TabIndex = 0;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(17, 201);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(353, 22);
            this.textBox.TabIndex = 3;
            // 
            // DelBtn
            // 
            this.DelBtn.Location = new System.Drawing.Point(17, 241);
            this.DelBtn.Name = "DelBtn";
            this.DelBtn.Size = new System.Drawing.Size(95, 23);
            this.DelBtn.TabIndex = 4;
            this.DelBtn.Text = "Delete";
            this.DelBtn.UseVisualStyleBackColor = true;
            this.DelBtn.Click += new System.EventHandler(this.DelBtn_Click);
            // 
            // AdBtn
            // 
            this.AdBtn.Location = new System.Drawing.Point(132, 241);
            this.AdBtn.Name = "AdBtn";
            this.AdBtn.Size = new System.Drawing.Size(100, 23);
            this.AdBtn.TabIndex = 5;
            this.AdBtn.Text = "Add";
            this.AdBtn.UseVisualStyleBackColor = true;
            this.AdBtn.Click += new System.EventHandler(this.AdBtn_Click);
            // 
            // SaceBtn
            // 
            this.SaceBtn.Location = new System.Drawing.Point(252, 241);
            this.SaceBtn.Name = "SaceBtn";
            this.SaceBtn.Size = new System.Drawing.Size(118, 23);
            this.SaceBtn.TabIndex = 6;
            this.SaceBtn.Text = "Save and close";
            this.SaceBtn.UseVisualStyleBackColor = true;
            this.SaceBtn.Click += new System.EventHandler(this.SaceBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Enter a new value";
            // 
            // ListEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 284);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SaceBtn);
            this.Controls.Add(this.AdBtn);
            this.Controls.Add(this.DelBtn);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.Lpredictions);
            this.Name = "ListEditorForm";
            this.Text = "Edit the list of predictions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button DelBtn;
        private System.Windows.Forms.Button AdBtn;
        public System.Windows.Forms.ListBox Lpredictions;
        private System.Windows.Forms.Button SaceBtn;
        private System.Windows.Forms.Label label1;
    }
}