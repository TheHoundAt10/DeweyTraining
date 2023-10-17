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
using System.Threading;

// Colby van Staden ST10081889 POE Part 2

namespace DeweyTraining
{
    public partial class IdentifyAreasPage : Form
    {
        private static readonly string resourceName = "DeweyTraining.Welcome to fantasy land.mp3"; // Replace with your resource name
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static Stream mp3Stream;
        private static IWavePlayer waveOutDevice;
        private static WaveStream waveStream;
        Dictionary<string, string> deweyDict0 = new Dictionary<string, string>
        {
            { "000", "General Works" },
            { "100", "Philosophy and Psychology" },
            { "200", "Religion" },
            { "300", "Social Sciences" },
            { "400", "Language" },
            { "500", "Natural Sciences and Mathematics" },
            { "600", "Technology (Applied Sciences)" },
            { "700", "Arts and Recreation" },
            { "800", "Literature" },
            { "900", "History and Geography" }
        };
        Dictionary<string, string> deweyDict1 = new Dictionary<string, string>
        {
            { "General Works", "000" },
            { "Philosophy and Psychology", "100" },
            { "Religion", "200" },
            { "Social Sciences", "300" },
            { "Language", "400" },
            { "Natural Sciences and Mathematics", "500" },
            { "Technology (Applied Sciences)", "600" },
            { "Arts and Recreation", "700" },
            { "Literature", "800" },
            { "History and Geography", "900" }
        };
        string leftChoice;
        string rightChoice;
        int score;
        string pairings = "Correct Pairings:\n\n" +
            "000 - General Works\n" +
            "100 - Philosophy and Psychology\n" +
            "200 - Religion\n" +
            "300 - Social Sciences\n" +
            "400 - Language\n" +
            "500 - Natural Sciences and Mathematics\n" +
            "600 - Technology (Applied Sciences)\n" +
            "700 - Arts and Recreation\n" +
            "800 - Literature\n" +
            "900 - History and Geography";

        public IdentifyAreasPage()
        {
            InitializeComponent();
            SetBackgroundImage();
        }

        public static void PlayBackgroundMusic()
        {
            mp3Stream = assembly.GetManifestResourceStream(resourceName);

            if (mp3Stream != null)
            {
                waveOutDevice = new WaveOutEvent();
                waveStream = new Mp3FileReader(mp3Stream);
                waveOutDevice.Init(waveStream);
                waveOutDevice.Play();
            }
        }

        public static void StopBackgroundMusic()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
            }

            if (waveStream != null)
            {
                waveStream.Close();
                waveStream.Dispose();
            }

            if (mp3Stream != null)
            {
                mp3Stream.Close();
                mp3Stream.Dispose();
            }
        }

        private bool CheckAnswer(string leftChoice, string rightChoice)
        {
            // Check if leftChoice is a key in deweyDict0.
            if (deweyDict0.ContainsKey(leftChoice))
            {
                // Check if the corresponding value in deweyDict0 matches rightChoice.
                string correctDescription = deweyDict0[leftChoice];
                if (correctDescription == rightChoice)
                {
                    return true; // The user's answer is correct.
                }
            }
            // If leftChoice is not found in deweyDict0, check deweyDict1.
            else if (deweyDict1.ContainsKey(leftChoice))
            {
                // Check if the corresponding value in deweyDict1 matches rightChoice.
                string correctCallNumber = deweyDict1[leftChoice];
                if (correctCallNumber == rightChoice)
                {
                    return true; // The user's answer is correct.
                }
            }

            // If neither dictionary contains leftChoice or the values don't match, return false.
            return false;
        }

        private void PopulateColumns()
        {
            ResetButtons();
            Random random = new Random();

            // Decide randomly whether call numbers are on the left (0) or right (1).
            int callNumberSide = random.Next(2);

            // Create lists for left items (4 items) and right answers (7 items).
            List<string> leftItems = new List<string>();
            List<string> rightAnswers = new List<string>();


            if (callNumberSide == 0)
            {
                // Left side is for call numbers.
                leftItems = GetRandomItems(deweyDict0.Keys.ToList(), 4, random);

                // Ensure that each item on the left has its correct answer on the right.
                foreach (var key in leftItems)
                {
                    string corresponding = deweyDict0[key];
                    rightAnswers.Add(corresponding);
                }
                // Add 3 more random values from the dictionary to rightAnswers.
                var remainingDescriptions = deweyDict0.Values.Except(rightAnswers).ToList();
                rightAnswers.AddRange(GetRandomItems(remainingDescriptions, 3, random));

                // Shuffle the order of items in rightAnswers.
                rightAnswers = rightAnswers.OrderBy(x => random.Next()).ToList();
            }
            else
            {
                // Left side is for descriptions.
                leftItems = GetRandomItems(deweyDict1.Keys.ToList(), 4, random);

                // Ensure that each item on the left has its correct answer on the right.
                foreach (var key in leftItems)
                {
                    string corresponding = deweyDict1[key];
                    rightAnswers.Add(corresponding);
                }
                // Add 3 more random values from the dictionary to rightAnswers.
                var remainingDescriptions = deweyDict1.Values.Except(rightAnswers).ToList();
                rightAnswers.AddRange(GetRandomItems(remainingDescriptions, 3, random));

                // Shuffle the order of items in rightAnswers.
                rightAnswers = rightAnswers.OrderBy(x => random.Next()).ToList();
            }


            string[] leftButtonNames = { "btnLeft1", "btnLeft2", "btnLeft3", "btnLeft4" };
            string[] rightButtonNames = { "btnRight1", "btnRight2", "btnRight3", "btnRight4", "btnRight5", "btnRight6", "btnRight7" };

                // Assign content to left buttons
                for (int i = 0; i < leftButtonNames.Length; i++)
                {
                    Button leftButton = this.Controls.Find(leftButtonNames[i], true).FirstOrDefault() as Button;

                    if (leftButton != null)
                    {
                        leftButton.Text = leftItems[i];
                    }
                }

                // Assign content to right buttons
                for (int i = 0; i < rightButtonNames.Length; i++)
                {
                    Button rightButton = this.Controls.Find(rightButtonNames[i], true).FirstOrDefault() as Button;

                    if (rightButton != null)
                    {
                        rightButton.Text = rightAnswers[i];
                    }
                }
        }

        private List<T> GetRandomItems<T>(List<T> sourceList, int count, Random random)
        {
            List<T> randomItems = new List<T>();
            List<T> tempList = new List<T>(sourceList);

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(tempList.Count);
                randomItems.Add(tempList[index]);
                tempList.RemoveAt(index);
            }

            return randomItems;
        }

        private void ResetButtons()
        {
            btnLeft1.BackColor = Color.DimGray;
            btnLeft2.BackColor = Color.DimGray;
            btnLeft3.BackColor = Color.DimGray;
            btnLeft4.BackColor = Color.DimGray;

            btnRight1.BackColor = Color.DimGray;
            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;

            btnLeft1.Enabled = true;
            btnLeft2.Enabled = true;
            btnLeft3.Enabled = true;
            btnLeft4.Enabled = true;

            btnRight1.Enabled = true;
            btnRight2.Enabled = true;
            btnRight3.Enabled = true;
            btnRight4.Enabled = true;
            btnRight5.Enabled = true;
            btnRight6.Enabled = true;
            btnRight7.Enabled = true;
        }

        private void IdentifyAreasPage_Load(object sender, EventArgs e)
        {
            PlayBackgroundMusic();

            string rules = "Game Rules:\n\n" +
            "1. Objective:\n" +
            "   - Match the call numbers to their correct descriptions.\n\n" +
            "2. How to Play:\n" +
            "   - Click on a call number/description in the left column and match it to the corresponding call number/description on the right.\n" +
            "   - Click the check button each time you think you've selected the correct pair.\n\n" +
            "3. Scoring:\n" +
            "   - Each correct pairing earns you a point.\n" +
            "   - If you match an incorrect pairing, the game is over and you may start again should you wish to do so.\n\n" +
            "4. Prizes:\n" +
            "   - Bronze Prize: 4 to 7 points\n" +
            "   - Silver Prize: 8 to 11 points\n" +
            "   - Gold Prize: 12 points or more\n\n" +
            "Click OK to see the correct pairings (This will be your last time seeing them until the game is over!)";

            MessageBox.Show(rules, "Game Rules", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(pairings, "Pairings", MessageBoxButtons.OK, MessageBoxIcon.Information);

            flpIdAr.BackColor = Color.FromArgb(128, 0, 0, 0);
            PopulateColumns();
        }

        private void SetBackgroundImage()
        {
            // Get the namespace of the project
            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;

            // Specify the image name
            string imageName = "identifyAreasBack.jpg";

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

        private void btnQuit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to quit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            StopBackgroundMusic();

            this.Close();

            // Open the MainPage form
            MainPage mainPage = new MainPage();
            mainPage.Show();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            bool twoSelected = TwoButtonsSelected();

            if (twoSelected) // Make sure two options are selected so the app can grade.
            {
                bool isCorrect = CheckAnswer(leftChoice, rightChoice);
                if (isCorrect)
                {
                    DisableSelectedButtons();
                    score++; // Increment the score by 1 for a correct answer.
                    rtbScore.Text = "Score: " + score; // Update the score in the RichTextBox.
                    if (score % 4 == 0)
                    {
                        PopulateColumns();
                    }
                }
                else
                {
                    GameOver();
                }
            } else
            {
                MessageBox.Show("Please select both a call number and a description.", "Reminder");
            }
            
        }

        private bool TwoButtonsSelected()
        {
            List<Button> leftButtons = new List<Button> { btnLeft1, btnLeft2, btnLeft3, btnLeft4 };
            List<Button> rightButtons = new List<Button> { btnRight1, btnRight2, btnRight3, btnRight4, btnRight5, btnRight6, btnRight7 };

            bool leftSelected = false;
            bool rightSelected = false;

            // Check if one button on the left side has LightSeaGreen.
            foreach (var button in leftButtons)
            {
                if (button.BackColor == Color.LightSeaGreen)
                {
                    leftSelected = true;
                    break; // Exit the loop if a left button is selected.
                }
            }

            // Check if one button on the right side has LightSeaGreen.
            foreach (var button in rightButtons)
            {
                if (button.BackColor == Color.LightSeaGreen)
                {
                    rightSelected = true;
                    break; // Exit the loop if a right button is selected.
                }
            }

            // Return true if both a left and a right button are selected.
            return leftSelected && rightSelected;
        }

        private void GameOver()
        {
            rtbScore.Text = "Game Over"; // Display "Game Over" to the user.
            SystemSounds.Beep.Play();

            btnLeft1.Enabled = false;
            btnLeft2.Enabled = false;
            btnLeft3.Enabled = false;
            btnLeft4.Enabled = false;

            btnRight1.Enabled = false;
            btnRight2.Enabled = false;
            btnRight3.Enabled = false;
            btnRight4.Enabled = false;
            btnRight5.Enabled = false;
            btnRight6.Enabled = false;
            btnRight7.Enabled = false;

            btnLeft1.BackColor = Color.Red;
            btnLeft2.BackColor = Color.Red;
            btnLeft3.BackColor = Color.Red;
            btnLeft4.BackColor = Color.Red;

            btnRight1.BackColor = Color.Red;
            btnRight2.BackColor = Color.Red;
            btnRight3.BackColor = Color.Red;
            btnRight4.BackColor = Color.Red;
            btnRight5.BackColor = Color.Red;
            btnRight6.BackColor = Color.Red;
            btnRight7.BackColor = Color.Red;

            btnPlayAgain.Visible = true;
            btnCheck.Visible = false;

            if (score > 3 && score < 8)
            {
                MessageBox.Show("You earned Bronze!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (score > 7 && score < 12)
            {
                MessageBox.Show("You earned Silver!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (score > 11)
            {
                MessageBox.Show("You earned Gold!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            score = 0; // Set the score to zero to reset the game.
        }

        private void DisableSelectedButtons()
        {
            List<Button> leftButtons = new List<Button> { btnLeft1, btnLeft2, btnLeft3, btnLeft4 };
            List<Button> rightButtons = new List<Button> { btnRight1, btnRight2, btnRight3, btnRight4, btnRight5 ,btnRight6 ,btnRight7 };

            foreach (var button in leftButtons)
            {
                if (button.BackColor == Color.LightSeaGreen)
                {
                    button.Enabled = false;
                    button.BackColor = Color.DimGray;
                    button.Text = "";
                    break; 
                }
            }

            foreach (var button in rightButtons)
            {
                if (button.BackColor == Color.LightSeaGreen)
                {
                    button.Enabled = false;
                    button.BackColor = Color.DimGray;
                    button.Text = "";
                    break;
                }
            }
        }

        private void btnLeft1_Click(object sender, EventArgs e)
        {
            btnLeft1.BackColor = Color.LightSeaGreen;
            leftChoice = btnLeft1.Text;
            
            btnLeft2.BackColor = Color.DimGray;
            btnLeft3.BackColor = Color.DimGray;
            btnLeft4.BackColor = Color.DimGray;
        }

        private void btnLeft2_Click(object sender, EventArgs e)
        {
            btnLeft2.BackColor = Color.LightSeaGreen;
            leftChoice = btnLeft2.Text;

            btnLeft1.BackColor = Color.DimGray;
            btnLeft3.BackColor = Color.DimGray;
            btnLeft4.BackColor = Color.DimGray;
        }

        private void btnLeft3_Click(object sender, EventArgs e)
        {
            btnLeft3.BackColor = Color.LightSeaGreen;
            leftChoice = btnLeft3.Text;

            btnLeft2.BackColor = Color.DimGray;
            btnLeft1.BackColor = Color.DimGray;
            btnLeft4.BackColor = Color.DimGray;
        }

        private void btnLeft4_Click(object sender, EventArgs e)
        {
            btnLeft4.BackColor = Color.LightSeaGreen;
            leftChoice = btnLeft4.Text;

            btnLeft2.BackColor = Color.DimGray;
            btnLeft3.BackColor = Color.DimGray;
            btnLeft1.BackColor = Color.DimGray;
        }

        private void btnRight1_Click(object sender, EventArgs e)
        {
            btnRight1.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight1.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight2_Click(object sender, EventArgs e)
        {
            btnRight2.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight2.Text;

            btnRight1.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight3_Click(object sender, EventArgs e)
        {
            btnRight3.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight3.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight1.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight4_Click(object sender, EventArgs e)
        {
            btnRight4.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight4.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight1.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight5_Click(object sender, EventArgs e)
        {
            btnRight5.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight5.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight1.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight6_Click(object sender, EventArgs e)
        {
            btnRight6.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight6.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight1.BackColor = Color.DimGray;
            btnRight7.BackColor = Color.DimGray;
        }

        private void btnRight7_Click(object sender, EventArgs e)
        {
            btnRight7.BackColor = Color.LightSeaGreen;
            rightChoice = btnRight7.Text;

            btnRight2.BackColor = Color.DimGray;
            btnRight3.BackColor = Color.DimGray;
            btnRight4.BackColor = Color.DimGray;
            btnRight5.BackColor = Color.DimGray;
            btnRight6.BackColor = Color.DimGray;
            btnRight1.BackColor = Color.DimGray;
        }

        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            MessageBox.Show(pairings, "Pairings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            PopulateColumns();
            rtbScore.Text = "";

            btnCheck.Visible= true;
            btnPlayAgain.Visible= false;
        }
    }
}
//========================= EoF ============================================