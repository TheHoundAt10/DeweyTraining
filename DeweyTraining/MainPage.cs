using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Media;
using NAudio.Wave; //NuGet Import

// Colby van Staden ST10081889 POE Part 1

namespace DeweyTraining
{
    public partial class MainPage : Form
    {
        private IWavePlayer player; // Declare an IWavePlayer instance
        private string temporaryFilePath; // Store the path to the temporary audio file

        public MainPage()
        {
            InitializeComponent();
            SetBackgroundImage();
            InitializeMusic();
        }

        private void InitializeMusic()
        {
            player = new WaveOutEvent(); // Create a WaveOutEvent instance

            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;
            string resourceName = "DeweyTraining.Kevin MacLeod_ Monkeys Spinning Monkeys [1 HOUR]_mWo3woi9Uro.mp3";

            // Generate a random file name
            string randomFileName = Guid.NewGuid().ToString() + ".mp3";
            temporaryFilePath = Path.Combine(Path.GetTempPath(), randomFileName);


            // Load the embedded resource and save it to a temporary file
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (FileStream fileStream = new FileStream(temporaryFilePath, FileMode.Create))
            {
                resourceStream.CopyTo(fileStream);
            }

            // Initialize the player with the temporary audio file
            player.Init(new AudioFileReader(temporaryFilePath));
        }


        private void MainPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Stop the music when the form is closed
            player.Stop();
            player.Dispose();

            // Try to delete the temporary audio file, handling any exceptions
            try
            {
                File.Delete(temporaryFilePath);
            }
            catch (IOException ex)
            {
                // Handle the exception
                MessageBox.Show("Error deleting temporary audio file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
        }



        private void btnReplaceBooks_Click(object sender, EventArgs e)
        {
            // Stop the music when the button is clicked
            player.Stop();
            player.Dispose();

            
            this.Hide();

            // Open the ReplaceBooksPage form
            ReplaceBooksPage replaceBooksPage = new ReplaceBooksPage();
            replaceBooksPage.Show();
            MainPage mainPage = new MainPage();
            mainPage.Close();
        }



        private void MainPage_Load(object sender, EventArgs e)
        {
            player.Play();
        }

        private void SetBackgroundImage()
        {
            // Get the namespace of the project
            string namespaceName = Assembly.GetExecutingAssembly().GetName().Name;

            // Specify the image name
            string imageName = "mainPageBack.jpg";

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

        private void btnIdentify_Click(object sender, EventArgs e)
        {
            // Stop the music when the button is clicked
            player.Stop();
            player.Dispose();


            this.Hide();

            // Open the ReplaceBooksPage form
            IdentifyAreasPage identifyAreasPage = new IdentifyAreasPage();
            identifyAreasPage.Show();
            MainPage mainPage = new MainPage();
            mainPage.Close();
        }
    }
}

//oooooooooooooooooooooooooooooooooooooO EoF Ooooooooooooooooooooooooooooooooooooooooooooo