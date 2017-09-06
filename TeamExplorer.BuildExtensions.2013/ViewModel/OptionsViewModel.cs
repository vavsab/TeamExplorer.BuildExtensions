using System;
using System.Collections.Generic;
using BuildTree.Models;
using BuildTree.ViewModel;

namespace BuildTree.DialogCloser.ViewModel
{
	/// <summary>
	/// Viewmodel class that manages input states for user who has a first and a last name.
	/// </summary>
	public class OptionsViewModel : ViewModelBase
	{
		#region fields
		private DialogViewModelBase _openCloseView;
		#endregion fields

		#region Constructors
		/// <summary>
		/// Standard Constructor
		/// </summary>
		public OptionsViewModel()
		{
			OptionsModel = new OptionsModel();
			DisplayName = "Build Tree Options";
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public OptionsViewModel(OptionsViewModel copyThis)
		{
			if (copyThis == null) return;

			_openCloseView = new DialogViewModelBase(copyThis._openCloseView);
			OptionsModel = copyThis.OptionsModel;
			DisplayName = "Build Tree Options";
		}

		#endregion Constructors

		#region Properties
		/// <summary>
		/// Get property to expose elements necessary to evaluate user input
		/// when the user completes his input (eg.: clicks OK in a dialog).
		/// </summary>
		public DialogViewModelBase OpenCloseView
		{
			get
			{
				return _openCloseView;
			}

			private set
			{
				if (_openCloseView == value) return;
				_openCloseView = value;
				NotifyPropertyChanged(() => OpenCloseView);
			}
		}

		/// <summary>
		/// Get/set the build name split character
		/// </summary>
		public string SplitCharacter
		{
			get { return string.Format("{0}", OptionsModel.SplitCharacter); }

			set
			{
				OptionsModel.SplitCharacter = !string.IsNullOrEmpty(value) ? new char?(Convert.ToChar(value)) : null;
				NotifyPropertyChanged(() => SplitCharacter);
			}
		}

		public OptionsModel OptionsModel { get; private set; }

		#endregion Properties

		#region Methods
		/// <summary>
		/// Initilize input states such that user can input information
		/// with a view based GUI (eg.: dialog)
		/// </summary>
		public void InitDialogInputData()
		{
			OpenCloseView = new DialogViewModelBase();

			// Attach delegate method to validate user input on OK
			// Not setting this means that user input is never validated and view will always close on OK
			if (_openCloseView != null)
				_openCloseView.EvaluateInputData = ValidateData;
		}

		/// <summary>
		/// Delegate method that is call whenever a user OK'es or Cancels the view that is bound to <seealso cref="OpenCloseView"/>
		/// </summary>
		/// <param name="listMsgs"></param>
		/// <returns></returns>
		private bool ValidateData(out List<Msg> listMsgs)
		{
			listMsgs = new List<Msg>();

			if (string.IsNullOrEmpty(SplitCharacter) || SplitCharacter.Length != 1)
			{
				listMsgs.Add(new Msg(string.Format("Split Character must be a single valid character.")));
			}
			return !(listMsgs.Count > 0);
		}
		#endregion Methods
	}
}
