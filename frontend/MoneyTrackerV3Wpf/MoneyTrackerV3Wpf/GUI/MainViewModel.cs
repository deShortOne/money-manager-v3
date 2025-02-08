using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MoneyTrackerV3Wpf.GUI;
public class MainViewModel : INotifyPropertyChanged
{
	public class Transaction
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public ObservableCollection<Transaction> Transactions { get; set; }

	public MainViewModel()
	{
		Transactions = new ObservableCollection<Transaction>
		{
			new Transaction
			{
				Id = 1,
				Name = "Test",
			},
		};
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}
