using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SecondDemo.Models;
using System;
using System.Xml.Linq;
using System.Linq;
namespace SecondDemo
{
    public partial class MainWindow : Window
    {
        int page = 0;
        int shownAmount;
        int f = 0;
        int s = 0;
        string sr = "";
        public MainWindow()
        {
            InitializeComponent();
            filtr.ItemsSource = PublicActions.Genders;
            Amount.SelectedIndex = 0;
            filtr.SelectedIndex = 0;
            sort.SelectedIndex = 0;
        }
        public void ChangePage()
        {
            ClientsList.ItemsSource = PublicActions.Clients.ToList().Skip(page * shownAmount).Take(shownAmount);
            show.Text = PublicActions.ShowmClientAmount;
            if (page == 0)
            {
                Back.IsEnabled = false;
            }
            else
            {
                Back.IsEnabled = true;
            }
            if ((page + 1) * shownAmount > PublicActions.Clients.Count - 1)
            {
                Forward.IsEnabled = false;
            }
            else
            {
                Forward.IsEnabled = true;
            }
        }

        private void PageButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "Back")
            {
                page--;
            }
            else
            {
                page++;
            }
            ChangePage();
        }

        private void ShownAmounChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0: shownAmount = 10; break;
                case 1: shownAmount = 50; break;
                case 2: shownAmount = 200; break;
            }
            page = 0;
            ChangePage();
        }

        private void SFSchanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Name == "filtr")
            {
                f = (sender as ComboBox).SelectedIndex;
            }
            else if ((sender as ComboBox).Name == "sort")
            {
                s = (sender as ComboBox).SelectedIndex;
            }
            page = 0;
            PublicActions.ClientsActions(s, f, sr);
            ChangePage();
        }

        private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
            sr = (sender as TextBox).Text;
            page = 0;
            PublicActions.ClientsActions(s, f, sr);
            ChangePage();
        }

        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new EditWindow().Show();
            this.Close();
        }

        private void Border_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            new EditWindow(Int32.Parse((sender as Border).Tag.ToString())).Show();
            this.Close();
        }

        private void ListBox_SelectionChanged_1(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if (ClientsList.SelectedIndex != -1)
            {
                DeleteButton.IsVisible = true;
            }
            else
            {
                DeleteButton.IsVisible = false;
            }
        }

        private void Delete(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PublicActions.PublicContext.Clients.Remove(ClientsList.SelectedItem as Client);
            PublicActions.PublicContext.SaveChanges();
            page = 0;
            f = 0;
            s = 0;
            PublicActions.ClientsActions(s, f, sr);
        }
    }
}