using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;
using System;
using Avalonia.Media.Imaging;
using SecondDemo.Models;
using System.Linq;
using System.Collections.Generic;
using SecondDemo.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace SecondDemo;

public partial class EditWindow : Window
{
    int index = -1;
    string checklastname = "";
    string checkname = "";
    string checkpat = "";
    string checkphpone = "";
    public string FileToCopy = "";
    public string FileName = "";
    public string OldFile = "";
    List<Models.Tag> tags = new List<Models.Tag>();
    List<Models.Tag> showntags = new List<Models.Tag>();
    public EditWindow()
    {
        InitializeComponent();
        Gender.ItemsSource = PublicActions.PublicContext.Genders.Select(g => g.Name);
        Id.Text = (PublicActions.PublicContext.Clients.OrderBy(c => c.Id).LastOrDefault().Id + 1).ToString();
    }
    public EditWindow(int i)
    {
        InitializeComponent();
        ImageButton.Content = "�������� ��������";
        Gender.ItemsSource = PublicActions.PublicContext.Genders.Select(g => g.Name);
        index = i;
        Client client = PublicActions.Clients.ToList().FirstOrDefault(c => c.Id == i);
        showntags = client.Tags.ToList();
        Extra.ItemsSource = client.Tags;
        Id.Text = client.Id.ToString();
        Lastname.Text = client.Lastname;
        Firstname.Text = client.Firstname;
        Patronim.Text = client.Patronymic;
        Email.Text = client.Email;
        Phone.Text = client.Phone;
        BDay.SelectedDate = DateTimeOffset.Parse(client.Birthday.ToString());
        Gender.SelectedIndex = PublicActions.PublicContext.Genders.ToList().IndexOf(PublicActions.PublicContext.Genders.ToList().First(g => g.Code == client.Gendercode));
        OldFile = client.Photopath;
        try
        {
            Image.Source = new Bitmap(Environment.CurrentDirectory + "/" + OldFile);
        }
        catch
        {
            Image.Source = new Bitmap(Environment.CurrentDirectory + "/ClientsPhotos/stock_photo.png");
        }
    }
    private async void AddImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        var TopLevel = await openFileDialog.ShowAsync(this);
        FileToCopy = string.Join("", TopLevel);
        if (FileToCopy != "")
        {
            int pos = FileToCopy.LastIndexOf('.');
            string str = FileToCopy.Substring(pos, FileToCopy.Length - pos);
            Image.Source = new Bitmap(FileToCopy);
            FileName = Guid.NewGuid().ToString() + str;
        }
        ImageButton.Content = "�������� ��������";
    }

    private void Comfirm(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Lastname.Text) || string.IsNullOrEmpty(Firstname.Text)
            || string.IsNullOrEmpty(Patronim.Text) || string.IsNullOrEmpty(Email.Text)
            || string.IsNullOrEmpty(Phone.Text) || Gender.SelectedIndex == null || !BDay.SelectedDate.HasValue || !Email.Text.Contains("@"))
        {
            return;
        }
        if (index == -1)
        {
            if (PublicActions.PublicContext.Clients.ToList().Where(t => t.Phone == Phone.Text).Count() != 0)
            {
                return;
            }
            Client client = new Client()
            {
                Id = PublicActions.PublicContext.Clients.OrderBy(c => c.Id).LastOrDefault().Id + 1,
                Lastname = Lastname.Text,
                Firstname = Firstname.Text,
                Patronymic = Patronim.Text,
                Email = Email.Text,
                Phone = Phone.Text,
                Birthday = DateOnly.FromDateTime(BDay.SelectedDate.Value.Date),
                Gendercode = PublicActions.PublicContext.Genders.ToList().FirstOrDefault(g => g.Name == PublicActions.PublicContext.Genders.Select(g => g.Name).ToList()[Gender.SelectedIndex]).Code,
                Photopath = $"ClientsPhotos/{FileName}"
            };
            PublicActions.PublicContext.Clients.Add(client);
            if (FileToCopy != "")
            {
                File.Copy(FileToCopy, $"{Environment.CurrentDirectory}/�������/{FileName}");
            }
            PublicActions.PublicContext.SaveChanges();

            foreach (Models.Tag tag in tags)
            {
                using (var context = new ShetininContext())
                {
                    var a = context.Clients.Include(z => z.Tags).FirstOrDefault(c => c.Id == index);
                    if (context.Tags.OrderBy(t => t.Id).LastOrDefault() == null)
                    {
                        tag.Id = 1;
                    }
                    else
                    {
                        tag.Id = context.Tags.OrderBy(t => t.Id).LastOrDefault().Id + 1;
                    }
                    a.Tags.Add(new Models.Tag() { Id = tag.Id, Title = tag.Title, Color = tag.Color });
                    context.SaveChanges();
                }
            }

        }
        else
        {
            Client client = PublicActions.Clients.ToList().FirstOrDefault(c => c.Id == index);
            client.Lastname = Lastname.Text;
            client.Firstname = Firstname.Text;
            client.Patronymic = Patronim.Text;
            client.Email = Email.Text;
            client.Phone = Phone.Text;
            client.Birthday = DateOnly.FromDateTime(BDay.SelectedDate.Value.Date);
            client.Gendercode = PublicActions.PublicContext.Genders.ToList().FirstOrDefault(g => g.Name == PublicActions.PublicContext.Genders.Select(g => g.Name).ToList()[Gender.SelectedIndex]).Code;
            if (FileToCopy != "")
            {
                client.Photopath = $"ClientsPhotos/{FileName}";
                File.Copy(FileToCopy, $"{Environment.CurrentDirectory}/ClientsPhotos/{FileName}");
                //FileInfo file = new FileInfo(OldFile);
                //file.Delete();
            }
            PublicActions.PublicContext.Clients.Update(client);
            PublicActions.PublicContext.SaveChanges();
            foreach (Models.Tag tag in tags)
            {
                using (var context = new ShetininContext())
                {
                    var a = context.Clients.Include(z => z.Tags).FirstOrDefault(c => c.Id == index);
                    if (context.Tags.OrderBy(t => t.Id).LastOrDefault() == null)
                    {
                        tag.Id = 1;
                    }
                    else
                    {
                        tag.Id = context.Tags.OrderBy(t => t.Id).LastOrDefault().Id + 1;
                    }
                    a.Tags.Add(new Models.Tag() { Id = tag.Id, Title = tag.Title, Color = tag.Color });
                    context.SaveChanges();
                }
            }
        }
        new MainWindow().Show();
        this.Close();
    }

    private void AddTag(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string a = string.Join("", ColorPic.Color.ToString().Skip(3));
        tags.Add(new Models.Tag() { Color = "#" + a, Title = TagName.Text, Id = 2 });
        showntags.Add(new Models.Tag() { Color = "#" + a, Title = TagName.Text, Id = 2 });
        TagName.Text = "";
        Extra.ItemsSource = showntags;

    }

    private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (!Regex.IsMatch((sender as TextBox).Text, @"^[a-zA-Z -]+$") || (sender as TextBox).Text.Count() > 50)
        {
            switch ((sender as TextBox).Name.ToString())
            {
                case "Lastname": (sender as TextBox).Text = checklastname; break;
                case "Firstname": (sender as TextBox).Text = checkname; break;
                case "Patronim": (sender as TextBox).Text = checkpat; break;
            };
        }
        else
        {
            switch ((sender as TextBox).Name.ToString())
            {
                case "Lastname":  checklastname = (sender as TextBox).Text; break;
                case "Firstname": checkname = (sender as TextBox).Text; break;
                case "Patronim": checkpat = (sender as TextBox).Text; break;
            };
        }
    }

    private void TextBox_TextChanged_1(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (!Regex.IsMatch((sender as TextBox).Text, @"^[1-9+() -]+$") )
        {
            (sender as TextBox).Text = checkphpone;
        }
        else
        {
            checkphpone = (sender as TextBox).Text;
        }
    }
}