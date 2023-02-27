namespace ROTten_Text;

public partial class MainPage : ContentPage
{
    string TextForEncDec, KeyOne, KeyTwo = "";

    public MainPage()
    {
        InitializeComponent();
    }

    //'How it works?' dropdown menu item
    async void OnHowItWorks(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new HowItWorksPage());

    }

    //'About' dropdown menu item
    async void OnAbout(object sender, EventArgs e)
    {
        string AppVersion = AppInfo.Current.VersionString;
        await DisplayAlert("About", "Version " + AppVersion + "\r\n\nROTten Text is a simple open-source app that encrypts/decrypts texts using the ROT method (rotate by fixed numbers of characters).\n\n" +
            "This app is licensed under GNU GPL v3.\r\n" +
            "The source code can be accessed on GitHub.\r\n\n" +
            "\nBy Moataz ZS. 🦆", "OK");
    }

    /////Editor text field/////
    void MessageBoxEncrypt(object sender, EventArgs e)
    {
        TextForEncDec = MessageBox.Text;
    }

    /////First key (number)/////
    void FrstKey(object sender, EventArgs e)
    {
        KeyOne = FirstKey.Text;

        if (int.TryParse(KeyOne, out int KeyOneInt) && KeyOneInt > 65535) //Turns the background color of the entry field to red once Key 1 exceeds 65,535
        {
            FirstKey.BackgroundColor = Colors.Red;
        }
        if (KeyOneInt <= 655355)
        {
            FirstKey.BackgroundColor = Colors.Transparent; //Known issues: Won't update when 'KeyOne' is below 65,535
        }
    }

    /////Second key (number)/////
    void ScndKey(object sender, EventArgs e)
    {
        KeyTwo = SecKey.Text;

        if (int.TryParse(KeyTwo, out int KeyTwoInt) && KeyTwoInt > 65535) //Turns the background color of the entry field to red once Key 2 exceeds 65,535
        {
            SecKey.BackgroundColor = Colors.Red;
        }
        if (KeyTwoInt <= 655355)
        {
            FirstKey.BackgroundColor = Colors.Transparent; //Known issues: Won't update when 'KeyTwo' is below 65,535
        }
    }



    /////Encryption button/////
    async void TxtEncryptBtn(object sender, EventArgs e)
    {
        ResulTxt.Text = "";

        //Checks if there is no text written in the MessageBox
        if (string.IsNullOrEmpty(TextForEncDec))
        {
            await DisplayAlert("Error", "Please write something first!", "OK");
            return;
        }

        //Checks if the first and second keys are entered or not
        else if (string.IsNullOrEmpty(KeyOne) || string.IsNullOrEmpty(KeyTwo))
        {
            await DisplayAlert("Error", "Please enter the first key AND the second key.", "OK");
            return;
        }


        //Checks if the first and second keys are valid numbers
        if (!int.TryParse(KeyOne, out int K1) || !int.TryParse(KeyTwo, out int K2))
        {
            await DisplayAlert("Error", "Please enter a valid number for the first key or the second key.", "OK");
            return;
        }

        //Checks if the sum of the first and second keys is over 65,535
        if (K1 + K2 > 65535)
        {
            await DisplayAlert("Error", "Please check that the sum of the first and second keys equals 65,535 or less.", "OK");
            return;
        }

        int TxtEncCharUni, KeysNText;
        string[] TxtToEncStringsArray = TextForEncDec.Select(c => c.ToString()).ToArray(); //Converts 'TextForEncDec' into an array of strings
                                                                                           //Could be optimized, but I'm too lazy to think :P | Known issues: Not really an issue, but when calling 'TxtToEncStringsArray' on a string containing an emoji, it will only return a high surrogate. Converting such characters to a different encoding can cause such issues.
        string[] EncryptedTxtStringsArray = new string[TxtToEncStringsArray.Length]; //Used to store the encrypted text

        for (int i = 0; i < TxtToEncStringsArray.Length; i++)
        {
            TxtEncCharUni = char.ConvertToUtf32(TxtToEncStringsArray[i], 0); //Converts 'TxtToEncStringsArray' to UTF-32 code point ---
                                                                             //I could convert this string to a byte array in UTF-8 format, but I want to cover more languages that use a large number of characters

            KeysNText = TxtEncCharUni + K1 + K2; //My ROT-NN implementation (N=0-65,535)
            if (KeysNText >= char.MinValue && KeysNText <= char.MaxValue) //Checks if 'KeysNText' is within the valid range of Unicode code points
            {
                EncryptedTxtStringsArray[i] = char.ConvertFromUtf32(KeysNText);
            }
            else
            {
                EncryptedTxtStringsArray[i] = KeysNText.ToString();
            }
        }
        string EncryptedTxt = string.Concat(EncryptedTxtStringsArray); //Concatenates all 'EncryptedTxtStringsArray' strings into a stingle string
        ResulTxt.Text = EncryptedTxt;
    }




    /////Decryption button/////
    async void TxtDecryptBtn(object sender, EventArgs e)
    {
        ResulTxt.Text = "";

        //Checks if there is no text written in the MessageBox
        if (string.IsNullOrEmpty(TextForEncDec))
        {
            await DisplayAlert("Error", "Please write something first!", "OK");
            return;
        }

        //Checks if the first and second keys are entered or not
        else if (string.IsNullOrEmpty(KeyOne) || string.IsNullOrEmpty(KeyTwo))
        {
            await DisplayAlert("Error", "Please enter the first key AND the second key.", "OK");
            return;
        }

        //Checks if the first and second keys are valid numbers
        if (!int.TryParse(KeyOne, out int K1) || !int.TryParse(KeyTwo, out int K2))
        {
            await DisplayAlert("Error", "Please enter a valid number for the first key or the second key.", "OK");
            return;
        }

        //Checks if the sum of the first and second keys is over 65,535
        if (K1 + K2 > 65535)
        {
            await DisplayAlert("Error", "Please check that the sum of the first and second keys equals 65,535 or less.", "OK");
            return;
        }

        int TxtDecCharUni, KeysNText;
        string[] TxtToDecStringsArray = TextForEncDec.Select(x => x.ToString()).ToArray(); //Converts 'TextForEncDec' into an array of strings
        string[] DecryptedTxtcStringsArray = new string[TextForEncDec.Length];

        for (int i = 0; i < TxtToDecStringsArray.Length; i++)
        {
            TxtDecCharUni = char.ConvertToUtf32(TxtToDecStringsArray[i], 0); //Converts 'TxtToDecStringsArray' to UTF-32 code point --- Otherwise, the same as I stated in 'TxtToEncStringsArray' above
            KeysNText = TxtDecCharUni - K1 - K2; //My ROT-NN implementation (N=0-65,535)
            if (KeysNText >= char.MinValue && KeysNText <= char.MaxValue) //Checks if 'KeysNText' is within the valid range of Unicode code points
            {
                DecryptedTxtcStringsArray[i] = char.ConvertFromUtf32(KeysNText);
            }
            else
            {
                DecryptedTxtcStringsArray[i] = KeysNText.ToString();
            }
        }

        string DecryptedTxt = string.Concat(DecryptedTxtcStringsArray); //Concatenates all 'DecryptedTxtcStringsArray' strings into a stingle string
        ResulTxt.Text = DecryptedTxt;
    }


    /////Results copy button/////
    private async void DecryptedTxtCopy(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(ResulTxt.Text);
    }


    /////Results share button/////
    public async void ResultsTxtShare(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(ResulTxt.Text))
        {
            await DisplayAlert("Error", "Please encrypt or decrypt something first", "OK");
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share Result",
            Text = ResulTxt.Text
        });
    }

    //Get Quacked! 🦆 :P
    int Quack = 0;
    async void OnQuack(object sender, EventArgs e)
    {
        Quack += 1;

        if (Quack % 2 == 0)
        {
            await DisplayAlert("🦆", "🦆 Do you love ducks? 🦆", "Yes!! 😃", "No :(");
        }
        else
        {
            await DisplayAlert("", "Quack!", "🦆");
        }
    }
}