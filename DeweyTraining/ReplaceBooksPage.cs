using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using NAudio.Wave;
using System.Text.RegularExpressions;
using DeweyGeneratorLibrary; //class library reference

// Colby van Staden ST10081889 POE Part 1

namespace DeweyTraining
{
    public partial class ReplaceBooksPage : Form
    {
        private IWavePlayer player; // Declare an IWavePlayer instance
        private string temporaryFilePath1; // Store the path to the temporary audio file
        public int moveCounter;
        List<string> deweyNumbersList = new List<string>();

        public ReplaceBooksPage()
        {
            InitializeComponent();
            SetBackgroundImage();
            InitializeMusic();
            moveCounter = 0;
            deweyNumbersList.Clear();
        }

        private void InitializeMusic()
        {
            player = new WaveOutEvent(); // Create a WaveOutEvent instance

            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;
            string resourceName = "DeweyTraining.Baby Elephant Walk 1 HOUR (128 kbps).mp3";

            // Generate a random file name
            string randomFileName = Guid.NewGuid().ToString() + ".mp3";
            temporaryFilePath1 = Path.Combine(Path.GetTempPath(), randomFileName);

            // Load the embedded resource and save it to a temporary file
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(temporaryFilePath1, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }

            // Initialize the player with the temporary audio file
            player.Init(new AudioFileReader(temporaryFilePath1));
        }

        private void ReplaceBooksPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop the music when the form is closed
            player.Stop();
            player.Dispose();

            // Try to delete the temporary audio file, handling any exceptions
            try
            {
                File.Delete(temporaryFilePath1);
            }
            catch (IOException ex)
            {
                // Handle the exception or display an error message
                MessageBox.Show("Error deleting temporary audio file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private void ReplaceBooksPage_Load(object sender, EventArgs e)
        {
            player.Play();
            // Show a message explaining the rules in a message dialog
            string rulesMessage = "Game Rules:\n\n" +
            "1. Objective:\n" +
            "   - Reorder the books by arranging their call numbers from smallest to biggest (largest at the bottom).\n\n" +
            "2. How to Play:\n" +
            "   - Click on the buttons next to a book to move it up and down the list.\n" +
            "   - Arrange the books in ascending order based on their call numbers.\n\n" +
            "3. Scoring:\n" +
            "   - Each book placed in the correct position earns you ten points.\n" +
            "   - Points are deducted for each move you make.\n\n" +
            "4. Prizes:\n" +
            "   - Bronze Prize: 45 to 59 points\n" +
            "   - Silver Prize: 60 to 74 points\n" +
            "   - Gold Prize: 75 points or more\n\n" +
            "5. Checking Your Work:\n" +
            "   - Click the \"Check\" button to see if you've sorted the books correctly.\n\n" +
            "Enjoy the game and aim for a high score!";


            MessageBox.Show(rulesMessage, "Game Rules", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Implementation of the class library
            Generator generator = new Generator();
            List<string> deweyNumbers = generator.GenerateRandomDeweyNumbers(10);
            deweyNumbersList = deweyNumbers;

            // Iterate through the TableLayoutPanel controls and set the labels
            for (int i = 0; i < deweyNumbers.Count; i++)
            {
                string deweyNumber = deweyNumbers[i];

                // Find the corresponding TableLayoutPanel by its name
                string tableLayoutPanelName = "tlp" + (i + 1); // i is zero-based, so add 1
                TableLayoutPanel tableLayoutPanel = Controls.Find(tableLayoutPanelName, true).FirstOrDefault() as TableLayoutPanel;

                if (tableLayoutPanel != null)
                {
                    // Find the corresponding label in the TableLayoutPanel
                    Label label = tableLayoutPanel.Controls.Find("lblCallNumber" + (i + 1), true).FirstOrDefault() as Label;

                    if (label != null)
                    {
                        // Set the label's text to the Dewey Decimal number
                        label.Text = deweyNumber;
                    }
                }
            }
        }


        private void btnCheck_Click(object sender, EventArgs e)
        {
            DisableUpDowns();
            btnCheck.Enabled = false;
            btnPlayAgain.Visible = true;
            BubbleSort(deweyNumbersList);
            int finalScore = EvaluateScore(deweyNumbersList) - moveCounter;

            rtbScore.Text = "Total Moves: " + moveCounter + Environment.NewLine;
            rtbScore.AppendText(Environment.NewLine);
            rtbScore.AppendText("Correct order:" + Environment.NewLine);
            rtbScore.AppendText(Environment.NewLine);

            foreach (string deweyNumber in deweyNumbersList)
            {
                rtbScore.AppendText(deweyNumber + Environment.NewLine);
            }

            rtbScore.AppendText(Environment.NewLine);
            rtbScore.AppendText("Final Score: " + finalScore);

            if (finalScore >= 45 && finalScore < 60)
            {
                MessageBox.Show("You earned Bronze!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (finalScore >= 60 && finalScore < 74)
            {
                MessageBox.Show("You earned Silver!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (finalScore >= 75)
            {
                MessageBox.Show("You earned Gold!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }


        private int EvaluateScore(List<string> deweyNumbersList)
        {
            int score = 0;

            for (int i = 0; i < deweyNumbersList.Count; i++)
            {
                string correctDewey = deweyNumbersList[i];
                string userDewey = GetDeweyFromRow(i);

                if (correctDewey == userDewey)
                {
                    // Award points for correct order
                    score += 10;
                }
            }

            return score;
        }

        
        private string GetDeweyFromRow(int rowIndex)
        {
            string userDewey = "";

            if (rowIndex == 0)
            {
                userDewey = lblCallNumber1.Text;
            }

            if (rowIndex == 1)
            {
                userDewey = lblCallNumber2.Text;
            }

            if (rowIndex == 2)
            {
                userDewey = lblCallNumber3.Text;
            }

            if (rowIndex == 3)
            {
                userDewey = lblCallNumber4.Text;
            }

            if (rowIndex == 4)
            {
                userDewey = lblCallNumber5.Text;
            }

            if (rowIndex == 5)
            {
                userDewey = lblCallNumber6.Text;
            }

            if (rowIndex == 6)
            {
                userDewey = lblCallNumber7.Text;
            }

            if (rowIndex == 7)
            {
                userDewey = lblCallNumber8.Text;
            }

            if (rowIndex == 8)
            {
                userDewey = lblCallNumber9.Text;
            }

            if (rowIndex == 9)
            {
                userDewey = lblCallNumber10.Text;
            }

            return userDewey;
        }


        private void BubbleSort(List<string> deweyNumbers)
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i < deweyNumbers.Count - 1; i++)
                {
                    if (CompareDeweyDecimal(deweyNumbers[i], deweyNumbers[i + 1]) > 0)
                    {
                        // Swap the elements
                        string temp = deweyNumbers[i];
                        deweyNumbers[i] = deweyNumbers[i + 1];
                        deweyNumbers[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);
        }

        private int CompareDeweyDecimal(string x, string y)
        {
            // Convert the Dewey Decimal strings to doubles for numerical comparison
            double dx, dy;
            if (double.TryParse(x, out dx) && double.TryParse(y, out dy))
            {
                return dx.CompareTo(dy);
            }

            // If parsing fails, fall back to string comparison
            return string.Compare(x, y);
        }

        private string GenerateRandomCallNumber()
        {
            Random rand = new Random();
            // Generate a random call number, you can customize this based on your needs
            int randomPart1 = rand.Next(100, 1000); 
            int randomPart2 = rand.Next(10, 100);   
            string callNumber = $"{randomPart1}.{randomPart2}";
            return callNumber;
        }

        private string GenerateBookNumber()
        {
            Random rand = new Random();
            int bookNum = rand.Next(1, 11);
            string bookNo = bookNum.ToString();
            return bookNo;
        }

        private void DisableUpDowns()
        {
            btnUp2.Enabled = false;
            btnUp3.Enabled = false;
            btnUp4.Enabled = false;
            btnUp5.Enabled = false;
            btnUp6.Enabled = false;
            btnUp7.Enabled = false;
            btnUp8.Enabled = false;
            btnUp9.Enabled = false;
            btnUp10.Enabled = false;
            btnDown.Enabled = false;
            btnDown2.Enabled = false;
            btnDown3.Enabled = false;
            btnDown4.Enabled = false;
            btnDown5.Enabled = false;
            btnDown6.Enabled = false;
            btnDown7.Enabled = false;
            btnDown8.Enabled = false;
            btnDown9.Enabled = false;
        }

        private void EnableUpDowns()
        {
            btnUp2.Enabled = true;
            btnUp3.Enabled = true;
            btnUp4.Enabled = true;
            btnUp5.Enabled = true;
            btnUp6.Enabled = true;
            btnUp7.Enabled = true;
            btnUp8.Enabled = true;
            btnUp9.Enabled = true;
            btnUp10.Enabled = true;
            btnDown.Enabled = true;
            btnDown2.Enabled = true;
            btnDown3.Enabled = true;
            btnDown4.Enabled = true;
            btnDown5.Enabled = true;
            btnDown6.Enabled = true;
            btnDown7.Enabled = true;
            btnDown8.Enabled = true;
            btnDown9.Enabled = true;
        }

        private void SetBackgroundImage()
        {
            // Get the namespace of the project
            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;

            // Specify the image name
            string imageName = "replaceBooksBack.jpg";

            // Load the embedded image
            Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(namespaceName + "." + imageName);

            if (imageStream != null)
            {
                // Create a Bitmap from the image stream
                Bitmap backgroundImage = new Bitmap(imageStream);

                // Set the background image of the form
                this.BackgroundImage = backgroundImage;

                this.BackgroundImageLayout = ImageLayout.Stretch;

                imageStream.Dispose();
            }
            else
            {
                MessageBox.Show("Background image not found in resources.");
            }
        }

        private void btnUp2_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm2.Image;
            string currentNum = lblCallNumber2.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm2.Image = pbxBookIm.Image;
            lblCallNumber2.Text = lblCallNumber1.Text;

            // Set the row above to the temporary values
            pbxBookIm.Image = currentImage;
            lblCallNumber1.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp3_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm3.Image;
            string currentNum = lblCallNumber3.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm3.Image = pbxBookIm2.Image;
            lblCallNumber3.Text = lblCallNumber2.Text;

            // Set the row above to the temporary values
            pbxBookIm2.Image = currentImage;
            lblCallNumber2.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp4_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm4.Image;
            string currentNum = lblCallNumber4.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm4.Image = pbxBookIm3.Image;
            lblCallNumber4.Text = lblCallNumber3.Text;

            // Set the row above to the temporary values
            pbxBookIm3.Image = currentImage;
            lblCallNumber3.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp5_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm5.Image;
            string currentNum = lblCallNumber5.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm5.Image = pbxBookIm4.Image;
            lblCallNumber5.Text = lblCallNumber4.Text;

            // Set the row above to the temporary values
            pbxBookIm4.Image = currentImage;
            lblCallNumber4.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp6_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm6.Image;
            string currentNum = lblCallNumber6.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm6.Image = pbxBookIm5.Image;
            lblCallNumber6.Text = lblCallNumber5.Text;

            // Set the row above to the temporary values
            pbxBookIm5.Image = currentImage;
            lblCallNumber5.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp7_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm7.Image;
            string currentNum = lblCallNumber7.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm7.Image = pbxBookIm6.Image;
            lblCallNumber7.Text = lblCallNumber6.Text;

            // Set the row above to the temporary values
            pbxBookIm6.Image = currentImage;
            lblCallNumber6.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp8_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm8.Image;
            string currentNum = lblCallNumber8.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm8.Image = pbxBookIm7.Image;
            lblCallNumber8.Text = lblCallNumber7.Text;

            // Set the row above to the temporary values
            pbxBookIm7.Image = currentImage;
            lblCallNumber7.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp9_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm9.Image;
            string currentNum = lblCallNumber9.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm9.Image = pbxBookIm8.Image;
            lblCallNumber9.Text = lblCallNumber8.Text;

            // Set the row above to the temporary values
            pbxBookIm8.Image = currentImage;
            lblCallNumber8.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnUp10_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm10.Image;
            string currentNum = lblCallNumber10.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm10.Image = pbxBookIm9.Image;
            lblCallNumber10.Text = lblCallNumber9.Text;

            // Set the row above to the temporary values
            pbxBookIm9.Image = currentImage;
            lblCallNumber9.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm.Image;
            string currentNum = lblCallNumber1.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm.Image = pbxBookIm2.Image;
            lblCallNumber1.Text = lblCallNumber2.Text;

            // Set the row above to the temporary values
            pbxBookIm2.Image = currentImage;
            lblCallNumber2.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown2_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm2.Image;
            string currentNum = lblCallNumber2.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm2.Image = pbxBookIm3.Image;
            lblCallNumber2.Text = lblCallNumber3.Text;

            // Set the row above to the temporary values
            pbxBookIm3.Image = currentImage;
            lblCallNumber3.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown3_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm3.Image;
            string currentNum = lblCallNumber3.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm3.Image = pbxBookIm4.Image;
            lblCallNumber3.Text = lblCallNumber4.Text;

            // Set the row above to the temporary values
            pbxBookIm4.Image = currentImage;
            lblCallNumber4.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown4_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm4.Image;
            string currentNum = lblCallNumber4.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm4.Image = pbxBookIm5.Image;
            lblCallNumber4.Text = lblCallNumber5.Text;

            // Set the row above to the temporary values
            pbxBookIm5.Image = currentImage;
            lblCallNumber5.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown5_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm5.Image;
            string currentNum = lblCallNumber5.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm5.Image = pbxBookIm6.Image;
            lblCallNumber5.Text = lblCallNumber6.Text;

            // Set the row above to the temporary values
            pbxBookIm6.Image = currentImage;
            lblCallNumber6.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown6_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm6.Image;
            string currentNum = lblCallNumber6.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm6.Image = pbxBookIm7.Image;
            lblCallNumber6.Text = lblCallNumber7.Text;

            // Set the row above to the temporary values
            pbxBookIm7.Image = currentImage;
            lblCallNumber7.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown7_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm7.Image;
            string currentNum = lblCallNumber7.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm7.Image = pbxBookIm8.Image;
            lblCallNumber7.Text = lblCallNumber8.Text;

            // Set the row above to the temporary values
            pbxBookIm8.Image = currentImage;
            lblCallNumber8.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown8_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm8.Image;
            string currentNum = lblCallNumber8.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm8.Image = pbxBookIm9.Image;
            lblCallNumber8.Text = lblCallNumber9.Text;

            // Set the row above to the temporary values
            pbxBookIm9.Image = currentImage;
            lblCallNumber9.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void btnDown9_Click(object sender, EventArgs e)
        {
            // Store the current image and call number in temporary variables
            Image currentImage = pbxBookIm9.Image;
            string currentNum = lblCallNumber9.Text;

            // Set the current image and call number to the values of the row above
            pbxBookIm9.Image = pbxBookIm10.Image;
            lblCallNumber9.Text = lblCallNumber10.Text;

            // Set the row above to the temporary values
            pbxBookIm10.Image = currentImage;
            lblCallNumber10.Text = currentNum;

            SystemSounds.Hand.Play();
            moveCounter = moveCounter + 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Stop and dispose of the player
            if (player != null)
            {
                player.Stop();
                player.Dispose();
            }

            // Hide the form instead of closing it
            this.Close();

            // Open the MainPage form
            MainPage mainPage = new MainPage();
            mainPage.Show();
        }

        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            deweyNumbersList.Clear();
            PopulateLabels();
            EnableUpDowns();
            btnCheck.Enabled = true;
            rtbScore.Text = "";
            moveCounter = 0;
            
            btnPlayAgain.Visible = false;
        }

        private void PopulateLabels()
        {
            // Implementation of the class library
            Generator generator = new Generator();
            List<string> deweyNumbers = generator.GenerateRandomDeweyNumbers(10);
            deweyNumbersList = deweyNumbers;
            if (deweyNumbers.Count >= 10)
            {
                lblCallNumber1.Text = deweyNumbers[0];
                lblCallNumber2.Text = deweyNumbers[1];
                lblCallNumber3.Text = deweyNumbers[2];
                lblCallNumber4.Text = deweyNumbers[3];
                lblCallNumber5.Text = deweyNumbers[4];
                lblCallNumber6.Text = deweyNumbers[5];
                lblCallNumber7.Text = deweyNumbers[6];
                lblCallNumber8.Text = deweyNumbers[7];
                lblCallNumber9.Text = deweyNumbers[8];
                lblCallNumber10.Text = deweyNumbers[9];
            }
            
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to quit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
    }
}
//oooooooooooooooooooooooooooooooooooooO EoF Ooooooooooooooooooooooooooooooooooooooooooooo