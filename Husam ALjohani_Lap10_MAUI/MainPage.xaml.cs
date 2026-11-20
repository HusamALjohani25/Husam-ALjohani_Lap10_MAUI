using Microsoft.Data.SqlClient;

namespace Husam_ALjohani_Lap10_MAUI;

public partial class MainPage : ContentPage
{
    private readonly string _connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\husam_205\\OneDrive\\Documents\\DB_MAUI.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True";

    public MainPage()
    {
        InitializeComponent();
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        lblError.IsVisible = false;

        string username = txtUsername.Text?.Trim() ?? "";
        string password = txtPassword.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Please enter username and password.";
            lblError.IsVisible = true;
            return;
        }

        try
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            string sql = "SELECT Id, name, role FROM UsersAccounts WHERE name = @name AND pass = @pass";
            using SqlCommand comm = new SqlCommand(sql, con);
            comm.Parameters.AddWithValue("@name", username);
            comm.Parameters.AddWithValue("@pass", password);

            con.Open();
            using SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                int userId = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                string name = reader["name"]?.ToString() ?? username;
                string role = reader["role"]?.ToString() ?? "customer";

                Preferences.Set("UserId", userId);
                Preferences.Set("Name", name);
                Preferences.Set("Role", role);

                await Navigation.PushAsync(new Page1());
            }
            else
            {
                lblError.Text = "Invalid username or password.";
                lblError.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void BtnGoToRegister_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}