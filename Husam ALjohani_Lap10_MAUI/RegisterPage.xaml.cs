using Microsoft.Data.SqlClient;

namespace Husam_ALjohani_Lap10_MAUI;

public partial class RegisterPage : ContentPage
{
    private readonly string _connectionString =
"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\husam_205\\OneDrive\\Documents\\DB_MAUI.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True";
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        lblRegMessage.IsVisible = false;

        string username = txtRegUsername.Text?.Trim() ?? "";
        string password = txtRegPassword.Text?.Trim() ?? "";
        string confirmPassword = txtRegConfirmPassword.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            lblRegMessage.Text = "Please fill in all fields.";
            lblRegMessage.TextColor = Colors.IndianRed;
            lblRegMessage.IsVisible = true;
            return;
        }

        if (password != confirmPassword)
        {
            lblRegMessage.Text = "Passwords do not match.";
            lblRegMessage.TextColor = Colors.IndianRed;
            lblRegMessage.IsVisible = true;
            return;
        }

        try
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string checkSql = "SELECT COUNT(*) FROM UsersAccounts WHERE name = @name";
            using (SqlCommand checkComm = new SqlCommand(checkSql, con))
            {
                checkComm.Parameters.AddWithValue("@name", username);
                int exists = Convert.ToInt32(checkComm.ExecuteScalar());
                if (exists > 0)
                {
                    lblRegMessage.Text = "Username already exists.";
                    lblRegMessage.TextColor = Colors.IndianRed;
                    lblRegMessage.IsVisible = true;
                    return;
                }
            }

            string insertSql = "INSERT INTO UsersAccounts (name, pass, role) VALUES (@name, @pass, @role)";
            using (SqlCommand insertComm = new SqlCommand(insertSql, con))
            {
                insertComm.Parameters.AddWithValue("@name", username);
                insertComm.Parameters.AddWithValue("@pass", password);
                insertComm.Parameters.AddWithValue("@role", "customer");
                insertComm.ExecuteNonQuery();
            }

            Preferences.Set("Name", username);
            Preferences.Set("Role", "customer");

            await DisplayAlert("Success", "Account created successfully!", "OK");
            await Navigation.PushAsync(new Page1());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnBackToLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}