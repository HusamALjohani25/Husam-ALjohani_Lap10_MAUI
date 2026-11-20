using HusamALjohani_Lap10_MAUI.Models;
using Microsoft.Data.SqlClient;

namespace Husam_ALjohani_Lap10_MAUI;

public partial class Page1 : ContentPage
{
    private readonly string _connectionString =
"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\husam_205\\OneDrive\\Documents\\DB_MAUI.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True";
    private List<Book> _allBooks = new List<Book>();

    public Page1()
    {
        InitializeComponent();
        LoadBooks();
    }

    private void LoadBooks(string selectedCategory = "All")
    {
        try
        {
            _allBooks.Clear();

            using SqlConnection con = new SqlConnection(_connectionString);
            string sql = "SELECT Id, title, price, imgfile, info, category FROM BookShop";
            using SqlCommand comm = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                _allBooks.Add(new Book
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    Title = reader["title"]?.ToString() ?? "",
                    Price = reader["price"] != DBNull.Value ? Convert.ToInt32(reader["price"]) : 0,
                    ImageUrl = reader["imgfile"]?.ToString() ?? "",
                    info = reader["info"]?.ToString() ?? "",
                    Category = reader["category"]?.ToString() ?? "General",
                    Quantity = 1
                });
            }

            ApplyCategoryFilter(selectedCategory);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void ApplyCategoryFilter(string category)
    {
        string cleanCategory = category?.Trim() ?? "All";

        if (string.Equals(cleanCategory, "All", StringComparison.OrdinalIgnoreCase))
        {
            myListView.ItemsSource = _allBooks.ToList();
        }
        else
        {
            myListView.ItemsSource = _allBooks
                .Where(b => string.Equals(b.Category?.Trim(), cleanCategory, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    private void FilterCategory_Clicked(object sender, EventArgs e)
    {
        if (sender is Button clickedBtn)
        {
            if (clickedBtn.Parent is HorizontalStackLayout bar)
            {
                foreach (var child in bar.Children)
                {
                    if (child is Button b)
                    {
                        b.BackgroundColor = b == clickedBtn
                            ? Color.FromArgb("#2563EB")
                            : Color.FromArgb("#334155");
                    }
                }
            }

            ApplyCategoryFilter(clickedBtn.Text);
        }
    }

    private async void myListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Book selectedBook)
        {
            selectedBook.Quantity = 1;
            myListView.SelectedItem = null;
            await Navigation.PushAsync(new Page2(selectedBook));
        }
    }
    private async void AddBook_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Book selectedBook)
        {
            selectedBook.Quantity = 1;
            await Navigation.PushAsync(new Page2(selectedBook));
        }
    }
}