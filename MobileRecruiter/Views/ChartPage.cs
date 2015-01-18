﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Syncfusion.SfChart.XForms;
using FormSample.Helpers;
using System;

namespace FormSample
{

	using Xamarin.Forms;
	public class ChartPage : ContentPage
	{
		SfChart chart1; 
		List<DailyRateCalcuationTable> dailyRate;
		DailyRateDataModel model;

		public ChartPage ()
		{
			dailyRate = new List<DailyRateCalcuationTable>();
			model = new DailyRateDataModel();

			Label header = new Label
			{
				Text = "Pay Chart", BackgroundColor = Color.Black, FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center, // Center the text in the blue box.
				YAlign = TextAlignment.Center,
				HeightRequest = 30
			};

			Label description = new Label
			{
				Text="Please find the helpful guide below to show how much difference a Limited company option could make to " +
					"your contractor's take home pay.",
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand,
			};

			var grid = new Grid
			{
				ColumnSpacing = 20
			};
			grid.Children.Add(new Label { Text = "Daily Rate", BackgroundColor=Color.FromHex("cccccc"), TextColor=Color.FromHex("eef2f3") }, 0, 0); // Left, First element
			grid.Children.Add(new Label { Text = "Limited Company" , BackgroundColor=Color.FromHex("cccccc"), TextColor=Color.FromHex("eef2f3")}, 1, 0);
			grid.Children.Add(new Label { Text = "Umbrella Company" ,BackgroundColor=Color.FromHex("cccccc"), TextColor=Color.FromHex("eef2f3")}, 2, 0);

			ListView list= new ListView{};
			list.VerticalOptions  = LayoutOptions.EndAndExpand;
			list.HeightRequest = 500;
			list.ItemTemplate = new DataTemplate(typeof(DailyRateCell));
			list.ItemsSource = GenerateDailyRateTable();
			chart1= new SfChart();
			 

			var contactUsButton = new Button { Text = "Contact us",BackgroundColor = Color.FromHex("0d9c00"), TextColor = Color.White };
			contactUsButton.Clicked +=  (object sender, EventArgs e) => 
			{
				App.RootPage.NavigateTo("Contact us");
			};

			GenerateChart();

			var headerStackLayout = new StackLayout (){ 
				Children = {header},
				Orientation = StackOrientation.Vertical
			};

			var descriptionLayout = new StackLayout () {
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(Device.OnPlatform(5, 5, 5),0 , Device.OnPlatform(5, 5, 5), 0), //new Thickness(5,0,5,0),
				VerticalOptions = LayoutOptions.FillAndExpand, 
				HorizontalOptions = LayoutOptions.Fill,
				Children= {description}
			};

			var buttonLayout = new StackLayout (){ 
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(Device.OnPlatform(5, 5, 5),0 , Device.OnPlatform(5, 5, 5), 0), //new Thickness(5,0,5,0),
				VerticalOptions = LayoutOptions.FillAndExpand, 
				HorizontalOptions = LayoutOptions.Fill,
				Children= {contactUsButton}
			};

			var layout = new StackLayout
			{
				// Children = {headerStackLayout,descriptionLayout,grid, list, chart1,buttonLayout},
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.Fill,
				Orientation = StackOrientation.Vertical
			};

			var listLayout = new StackLayout () {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation=StackOrientation.Vertical
			};

			var chartLayout = new StackLayout () {
				VerticalOptions = LayoutOptions.EndAndExpand,
				Orientation=StackOrientation.Vertical
			};

			listLayout.Children.Add (list);
			chartLayout.Children.Add (chart1);

			layout.Children.Add (headerStackLayout);
			layout.Children.Add (descriptionLayout);
			layout.Children.Add (grid);
			layout.Children.Add (listLayout);
			layout.Children.Add (chartLayout);
			layout.Children.Add (buttonLayout);
			Content = new ScrollView (){ Content = layout };
		}

		private List<DailyRateCalcuationTable> GenerateDailyRateTable()
		{

			FormSample.PayTableDatabase d = new  FormSample.PayTableDatabase();
			for (double rate = 100; rate <= 500; rate += 50)
			{
				double weeklyExpense = 50;
				var grossPay = rate * 5;
				var taxablePay = grossPay - weeklyExpense;
				double takeHomePayLimited = 0;
				var payData = d.GetPayTableTaxablePay(taxablePay); //TODO: taxable pay
				if (payData != null)
				{
					var netPay = payData.TakeHomeLimited;
					takeHomePayLimited = netPay + weeklyExpense;
					var percentLimited = (takeHomePayLimited / grossPay) * 100;
				}

				double takeHomeUmbrella = 0;
				payData = d.GetPayTableTaxablePay(grossPay);
				if (payData != null)
				{
					takeHomeUmbrella = payData.TakeHomeUmbrella;
					var percentUmbrella = (takeHomeUmbrella / grossPay) * 100;

				}

				dailyRate.Add(new DailyRateCalcuationTable(){ 
					DailyRate = rate,
					LimitedCompany = takeHomePayLimited,
					UmbrellaCompany= takeHomeUmbrella
				});
				model.SetLimitedCompanyData(rate.ToString(), takeHomePayLimited);
				model.SetUmbrellaCompanyData(rate.ToString(), takeHomeUmbrella);
			}
			return dailyRate;
		}
		private void GenerateChart()
		{
			chart1.Title=new ChartTitle(){Text="Your weekly pay"};
			chart1.Title.Font = Font.OfSize("Arial", 10);
			chart1.WidthRequest = 200;
			chart1.HeightRequest = 500;

			//Initializing Primary Axis
			Syncfusion.SfChart.XForms.CategoryAxis primaryAxis=new Syncfusion.SfChart.XForms.CategoryAxis();
			primaryAxis.Title = new ChartAxisTitle(){Text= "Daily Rate"};;
			chart1.PrimaryAxis=primaryAxis;
 

			chart1.Series.Add(new Syncfusion.SfChart.XForms.ColumnSeries()
				{
					ItemsSource = model.limitedCompanyTax,
					YAxis=new NumericalAxis(){IsVisible=true },
					IsVisibleOnLegend =true  ,
					Label="Limited"
				});
			chart1.Series.Add(new Syncfusion.SfChart.XForms.ColumnSeries()
				{
					ItemsSource = model.umbrallaCompanyTax,
					YAxis=new NumericalAxis(){IsVisible=false },
					IsVisibleOnLegend =true,
					Label="Umbrella"
				});


			//Adding Chart Legend for the Chart
			chart1.Legend = new ChartLegend() 
			{ 
				IsVisible = true, 

				DockPosition= Syncfusion.SfChart.XForms.LegendPlacement.Bottom ,
				LabelStyle = new ChartLegendLabelStyle(){Font = Font.OfSize("Arial", 10) }
			};
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			GC.Collect ();
		}
	}

	public class DailyRateCalcuationTable
	{
		public double DailyRate {get;set;}
		public double LimitedCompany {get;set;}
		public double UmbrellaCompany { get; set; }
	}

	public class DailyRateDataModel
	{
		// public ObservableCollection<ChartDataPoint> dailyRate;
		public ObservableCollection<ChartDataPoint> limitedCompanyTax;
		public ObservableCollection<ChartDataPoint> umbrallaCompanyTax;

		public DailyRateDataModel()
		{
			// dailyRate =new ObservableCollection<ChartDataPoint>();
			limitedCompanyTax = new ObservableCollection<ChartDataPoint>();
			umbrallaCompanyTax = new ObservableCollection<ChartDataPoint>();

		}
		public void SetLimitedCompanyData(string title, double value)
		{
			this.limitedCompanyTax.Add(new ChartDataPoint(title,value));
		}

		public void SetUmbrellaCompanyData(string title, double value)
		{
			this.umbrallaCompanyTax.Add(new ChartDataPoint(title, value));
		}
	}

	public class DailyRateCell : ViewCell
	{
		public DailyRateCell()
		{

			var nameLayout = CreateLayout();
			var viewLayout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				Children = { nameLayout },
			};
			viewLayout.BackgroundColor = MyContractorPage.counter % 2 == 0 ? Color.Silver: Color.Gray ;
			MyContractorPage.counter++;
			View = viewLayout;
		}
		private StackLayout CreateLayout()
		{
			var nameLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand };
			nameLabel.SetBinding(Label.TextProperty, new Binding("DailyRate"));
			nameLabel.WidthRequest = 130;
			nameLabel.TextColor = Color.Black;

			var limitedCompanyLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand };
			limitedCompanyLabel.SetBinding(Label.TextProperty, new Binding("LimitedCompany"));
			limitedCompanyLabel.WidthRequest = 130;
			limitedCompanyLabel.TextColor = Color.Black;

			var UmbrellaCompanyLabel = new Label { HorizontalOptions = LayoutOptions.FillAndExpand };
			UmbrellaCompanyLabel.SetBinding(Label.TextProperty, new Binding("UmbrellaCompany"));
			UmbrellaCompanyLabel.WidthRequest = 80;
			UmbrellaCompanyLabel.TextColor = Color.Black;

			var nameLayout = new StackLayout()
			{
				HorizontalOptions = LayoutOptions.StartAndExpand,
				Orientation = StackOrientation.Horizontal,
				Children = { nameLabel, limitedCompanyLabel, UmbrellaCompanyLabel }
			};
			return nameLayout;
		}

	}

}

