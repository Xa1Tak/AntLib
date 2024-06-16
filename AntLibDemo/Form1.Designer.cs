namespace AntLibDemo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            trainButton = new Button();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            encodeTextBox = new TextBox();
            decodeTextBox = new TextBox();
            encodeButton = new Button();
            decodeButton = new Button();
            changeButton = new Button();
            setModelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // trainButton
            // 
            trainButton.Location = new Point(22, 12);
            trainButton.Name = "trainButton";
            trainButton.Size = new Size(75, 23);
            trainButton.TabIndex = 0;
            trainButton.Text = "Train";
            trainButton.UseVisualStyleBackColor = true;
            trainButton.Click += trainButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(22, 53);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(280, 280);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(508, 53);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(280, 280);
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // encodeTextBox
            // 
            encodeTextBox.Location = new Point(22, 348);
            encodeTextBox.Multiline = true;
            encodeTextBox.Name = "encodeTextBox";
            encodeTextBox.Size = new Size(280, 90);
            encodeTextBox.TabIndex = 3;
            // 
            // decodeTextBox
            // 
            decodeTextBox.Location = new Point(508, 348);
            decodeTextBox.Multiline = true;
            decodeTextBox.Name = "decodeTextBox";
            decodeTextBox.Size = new Size(280, 90);
            decodeTextBox.TabIndex = 4;
            // 
            // encodeButton
            // 
            encodeButton.Location = new Point(364, 262);
            encodeButton.Name = "encodeButton";
            encodeButton.Size = new Size(75, 26);
            encodeButton.TabIndex = 5;
            encodeButton.Text = "Encode";
            encodeButton.UseVisualStyleBackColor = true;
            encodeButton.Click += encodeButton_Click;
            // 
            // decodeButton
            // 
            decodeButton.Location = new Point(364, 310);
            decodeButton.Name = "decodeButton";
            decodeButton.Size = new Size(75, 23);
            decodeButton.TabIndex = 6;
            decodeButton.Text = "Decode";
            decodeButton.UseVisualStyleBackColor = true;
            decodeButton.Click += decodeButton_Click;
            // 
            // changeButton
            // 
            changeButton.Location = new Point(364, 105);
            changeButton.Name = "changeButton";
            changeButton.Size = new Size(75, 26);
            changeButton.TabIndex = 7;
            changeButton.Text = "Change";
            changeButton.UseVisualStyleBackColor = true;
            changeButton.Click += changeButton_Click;
            // 
            // setModelButton
            // 
            setModelButton.Location = new Point(227, 12);
            setModelButton.Name = "setModelButton";
            setModelButton.Size = new Size(75, 23);
            setModelButton.TabIndex = 8;
            setModelButton.Text = "SetModel";
            setModelButton.UseVisualStyleBackColor = true;
            setModelButton.Click += setModelButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(setModelButton);
            Controls.Add(changeButton);
            Controls.Add(decodeButton);
            Controls.Add(encodeButton);
            Controls.Add(decodeTextBox);
            Controls.Add(encodeTextBox);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(trainButton);
            Name = "Form1";
            Text = "AntLibDemo";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button trainButton;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private TextBox encodeTextBox;
        private TextBox decodeTextBox;
        private Button encodeButton;
        private Button decodeButton;
        private Button changeButton;
        private Button setModelButton;
    }
}
