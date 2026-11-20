using HusamALjohani_Lap10_MAUI.Models;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Husam_ALjohani_Lap10_MAUI;

public partial class Page2 : ContentPage
{
 private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\USERS\\HUSAM_205\\ONEDRIVE\\DOCUMENTS\\DB_MAUI.MDF;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True;";

    static List<Book> BuyBooks = new List<Book>();

    public Page2(Book bk)
    {
        InitializeComponent();

        bool flage = false;
        foreach (var book in BuyBooks.ToList())
        {
            if (book.Title == bk.Title)
            {
                flage = true;
                break;
            }
        }

        if (!flage)
        {
            BuyBooks.Add(bk);
        }

        myListView.ItemsSource = null;
        myListView.ItemsSource = BuyBooks;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Page1());
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            string custname = Preferences.Get("Name", "Guest");

            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();

            string sql = "INSERT INTO bookorder (custname, orderdate, total) VALUES (@custname, @orderdate, 0)";
            using (SqlCommand comm = new SqlCommand(sql, con))
            {
                comm.Parameters.AddWithValue("@custname", custname);
                comm.Parameters.AddWithValue("@orderdate", DateTime.Today);
                comm.ExecuteNonQuery();
            }

            int ordid = 0;
            string getOrderSql = "SELECT TOP 1 Id FROM bookorder WHERE custname = @custname ORDER BY Id DESC";
            using (SqlCommand getOrderComm = new SqlCommand(getOrderSql, con))
            {
                getOrderComm.Parameters.AddWithValue("@custname", custname);
                object result = getOrderComm.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    ordid = Convert.ToInt32(result);
                }
            }

            int tot = 0;
            foreach (var book in BuyBooks.ToList())
            {
                if (book.Quantity > 0)
                {
                    string insertLineSql =
                        "INSERT INTO book_orderline (orderid, itemname, itemquantity, itemprice) VALUES (@orderid, @itemname, @itemquantity, @itemprice)";
                    using (SqlCommand lineComm = new SqlCommand(insertLineSql, con))
                    {
                        lineComm.Parameters.AddWithValue("@orderid", ordid);
                        lineComm.Parameters.AddWithValue("@itemname", book.Title);
                        lineComm.Parameters.AddWithValue("@itemquantity", book.Quantity);
                        lineComm.Parameters.AddWithValue("@itemprice", book.Price);
                        lineComm.ExecuteNonQuery();
                    }

                    string updateStockSql =
                        "UPDATE BookShop SET bookquantity = bookquantity - @qty WHERE title = @title";
                    using (SqlCommand stockComm = new SqlCommand(updateStockSql, con))
                    {
                        stockComm.Parameters.AddWithValue("@qty", book.Quantity);
                        stockComm.Parameters.AddWithValue("@title", book.Title);
                        stockComm.ExecuteNonQuery();
                    }

                    tot += book.Quantity * book.Price;
                }
            }

            string updateTotalSql = "UPDATE bookorder SET total = @total WHERE Id = @id";
            using (SqlCommand totalComm = new SqlCommand(updateTotalSql, con))
            {
                totalComm.Parameters.AddWithValue("@total", tot);
                totalComm.Parameters.AddWithValue("@id", ordid);
                totalComm.ExecuteNonQuery();
            }

            await DisplayAlert("Alert", "Thank You for your purchase you need to pay " + tot, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

