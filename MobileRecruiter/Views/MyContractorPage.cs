﻿using FormSample.Helpers;

namespace FormSample
{
	using FormSample.ViewModel;
	using System;
	using Xamarin.Forms;
	using Xamarin.Forms.Labs.Controls;

	public class MyContractorPage : ContentPage
	{
		public static int counter { get; set; }
		private ContractorViewModel contractorViewModel;
		private ContractorDataService dataService = new ContractorDataService();
		private ListView listView;
		private IProgressService progressService;

		public MyContractorPage()
		{
			BindingContext = new ContractorViewModel ();
			progressService = DependencyService.Get<IProgressService> ();
			contractorViewModel = new ContractorViewModel();
			counter = 1;

			var label = new Label{ Text = "My contractor",BackgroundColor = Color.Black,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center, // Center the text in the blue box.
				YAlign = TextAlignment.Center
			};



			listView = new ListView
			{
				RowHeight = 40
			};
			var grid = new Grid
			{
				ColumnSpacing = 100
			};
			grid.Children.Add(new Label { Text = "Contractor", TextColor=Color.Red }, 0, 0); // Left, First element
			grid.Children.Add(new Label { Text = "Date refered" ,TextColor=Color.Red}, 1, 0);

			var btnClearAllContractor = new Button { Text = "Clear all contractor", BackgroundColor = Color.FromHex("3b73b9"), TextColor = Color.White };
			btnClearAllContractor.SetBinding (Button.CommandProperty, ContractorViewModel.GotoDeleteAllContractorCommandPropertyName);

			btnClearAllContractor.Clicked += async (object sender, EventArgs e) => {
				try
				{
					var answer = await DisplayAlert("Confirm", "Do you wish to clear all item", "Yes", "No");
					if (answer)
					{
						progressService.Show();
						var result = await dataService.DeleteAllContractor(Settings.GeneralSettings);
						if(result != null)
						{
							progressService.Dismiss();
							listView.ItemsSource = this.contractorViewModel.contractorList;
						}
					}
				}
				catch
				{
					//progressService.Dismiss();
					DisplayAlert("Message", Utility.SERVERERRORMESSAGE, "OK");
				}
			};
			var contactUsButton = new Button { Text = "Contact Us", BackgroundColor = Color.FromHex("0d9c00"), TextColor = Color.White };
			contactUsButton.Clicked += delegate
			{
				App.RootPage.NavigateTo("Contact us");
			};

			var labelStakeLayout = new StackLayout (){ 
				Children = {label}
			};

			var controlStakeLayout = new StackLayout (){ 
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(Device.OnPlatform(5, 5, 5),0 , Device.OnPlatform(5, 5, 5), 0), //new Thickness(5,0,5,0),
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = { grid, listView}

			};

			var buttonLayout = new StackLayout (){ 
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(Device.OnPlatform(5, 5, 5),0 , Device.OnPlatform(5, 5, 5), 0), //new Thickness(5,0,5,0),
				Children= {btnClearAllContractor, contactUsButton}
			};

			var nameLayOut = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {labelStakeLayout,controlStakeLayout,buttonLayout}
			};

			Content = new StackLayout
			{
				Children ={ nameLayOut}
			};

			listView.ItemTapped += async (sender, args) =>
			{
				var contractor = args.Item as Contractor;
				if (contractor == null)
				{
					return;
				}
				var answer = await DisplayAlert("Confirm", "Do you wish to clear this item", "Yes", "No");
				if (answer)
				{
					progressService.Show();
					var result = await dataService.DeleteContractor(contractor.Id, Settings.GeneralSettings);
					if(result != null)
					{
						await this.contractorViewModel.DeleteContractor(contractor.Id);
						progressService.Dismiss();
						listView.ItemsSource = this.contractorViewModel.contractorList;
					}
				}

				listView.SelectedItem = null;
			};

		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			MessagingCenter.Subscribe<ContractorViewModel,string> (this, "msg", async(sender, args) => await this.DisplayAlert ("Confirm", args, "Yes", "No"));
			progressService.Show ();
			try
			{
				var x = DependencyService.Get<FormSample.Helpers.Utility.INetworkService>().IsReachable();
				if (!x) {
					progressService.Dismiss ();
					await DisplayAlert ("Message", Utility.NOINTERNETMESSAGE, "OK");
				} else {
					await this.contractorViewModel.BindContractor ();
					listView.ItemTemplate = new DataTemplate (typeof(ContractorCell));
					listView.ItemsSource = this.contractorViewModel.contractorList;
				}
			}
			catch(Exception) {
				progressService.Dismiss ();
				DisplayAlert ("Message", Utility.SERVERERRORMESSAGE, "OK");
			}
		}

		protected override  void OnDisappearing()
		{
			base.OnDisappearing ();
			progressService.Dismiss ();
			MessagingCenter.Unsubscribe<ContractorViewModel, string>(this, "msg");
			GC.Collect ();
		}
	}
}
