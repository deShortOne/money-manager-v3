using MoneyTracker.Contracts.Responses.Transaction;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MoneyTrackerV3Wpf.GUI;
public class MainViewModel : INotifyPropertyChanged
{
	public ObservableCollection<TransactionResponse> Transactions { get; set; }

	public MainViewModel()
	{
		Transactions = new ObservableCollection<TransactionResponse>
		{
			new(1, new(89, "Payee A"), 120.50M, new DateOnly(2023, 11, 13), new(80, "Category A"), new(69, "Account 1")),
			new(3, new(78, "Payee B"), 70.50M, new DateOnly(2024, 7, 25), new(81, "Category 2"), new(66, "Account B")),
		};
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}
