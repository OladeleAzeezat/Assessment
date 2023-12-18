using AssessmentMaui.Model;
using AssessmentMaui.Services;
using AssessmentMaui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssessmentMaui.ViewModels;
public partial class ItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _description;

    public event PropertyChangedEventHandler PropertyChanged;

    private ObservableCollection<Item> _items;
    public ObservableCollection<Item> Items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }
    }

    public ICommand AddCommand { get; private set; }
    public ICommand EditCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    readonly IAssessmentRepository assessment = new AssessmentService();
    public ItemViewModel()
    {
        // _itemService = new ItemService(); // Assuming ItemService is your backend service class
        AddCommand = new Command(async () => await Add());
        EditCommand = new Command<Item>(async (item) => await Edit());
        DeleteCommand = new Command<Item>(async (item) => await Delete(item));


        //new Command(Add);
        //new Command<Item>(Edit);
        //new Command<Item>(Delete);

        LoadItems();
    }

  
    private async Task LoadItems()
    {
        try
        {
            //var items =  assessment.GetItems();
            //var Items = new ObservableCollection<Item>(assessment.GetItems());
            IEnumerable<Item> items = await assessment.GetItemsAsync(); 
            Items = new ObservableCollection<Item>(items);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
        }

    }

    private async Task Add()
    {
        try
        {
            Item item = await assessment.AddItem(new Item
            {
                description = Description,
                itemName = Name
            });
            if (item != null)
            {
                if (Preferences.ContainsKey(nameof(App.item)))
                {
                    Preferences.Remove(nameof(App.item));
                }
                string itemDetails = JsonConvert.SerializeObject(item);
                Preferences.Set(nameof(App.item), itemDetails);
                App.item = item;
                //Items.Add(item);
                LoadItems();

            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Item not Added", "Ok");
                return;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return;
        }
    }

    private async Task Edit()
    {
        try
        {
            Item item = await assessment.AddItem(new Item
            {
                description = Description,
                itemName = Name
            });
            if (item != null)
            {
                Preferences.Clear(); // Clear all preferences
                string itemDetails = JsonConvert.SerializeObject(item);
                Preferences.Set(nameof(App.item), itemDetails);
                App.item = item;

                LoadItems();
            }
            else
            {
                await Shell.Current.DisplayAlert("Failed", "Failed to add item", "Ok");
            }            
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return;
        }
    }

    private async Task Delete(Item item)
    {
        try
        {
            // Assuming assessment.DeleteItemAsync is a method to delete the item
            bool isDeleted = await assessment.Delete(item.id);

            if (isDeleted)
            {
                await Shell.Current.DisplayAlert("Success", "Deleted Succesfully", "Ok");
                LoadItems();
            }
            else
            {
                await Shell.Current.DisplayAlert("Failed", "Failed to delete item", "Ok");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "An error occurred while deleting the item", "Ok");
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private async void OnPageAppearing(object sender, EventArgs e)
    {
        await LoadItems();
    }

    


}
//[RelayCommand]
//public async void Items()
//{
//    try
//    {
//        Item item = await assessment.GetAllItem( new ); // Assuming GetAllItem is asynchronous

//        if (item != null)
//        {
//            if (Preferences.ContainsKey(nameof(App.item)))
//            {
//                Preferences.Remove(nameof(App.item));
//            }

//            string itemDetails = JsonConvert.SerializeObject(item);
//            Preferences.Set(nameof(App.item), itemDetails);
//            App.item = item;
//        }
//        else
//        {
//            await Shell.Current.DisplayAlert("Error", "Empty", "Ok");
//            return;
//        }
//    }
//    catch (Exception ex)
//    {
//        // Handle exception or log it
//        Console.WriteLine($"Exception: {ex.Message}");
//        return;
//    }
//}


//[RelayCommand]
//public async void Items()
//{

//    //Item item = new Item();
//    try
//    {
//        Item item = assessment.GetAllItem();

//        if (item != null)
//        {
//            if (Preferences.ContainsKey(nameof(App.item)))
//            {
//                Preferences.Remove(nameof(App.item));
//            }


//            string itemDetails = JsonConvert.SerializeObject(item);
//            Preferences.Get(nameof(App.item), itemDetails);
//            App.item = item;
//        }
//        else
//        {
//            await Shell.Current.DisplayAlert("Error", "Empty", "Ok");
//            return;
//        }
//    }
//    catch (Exception ex) 
//    {
//        return ;
//    }
//}

//[RelayCommand]
//private void Add()
//{
//    // Implement logic to add a new item to the collection
//    // Example: Items.Add(new Item { ... });
//}

//[RelayCommand]
//private void Edit()
//{
//    // Implement logic to edit the selected item
//    // Example: SelectedItem.Name = "Updated Name";
//}

//[RelayCommand]
//private void Delete()
//{
//    // Implement logic to delete the selected item
//    // Example: Items.Remove(SelectedItem);
//}

