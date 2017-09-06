using System;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BuildTree.DialogCloser.ViewModel;

namespace BuildTree.DialogCloser.Converter
{
	[ValueConversion(typeof(Msg.MsgCategory), typeof(System.Windows.Media.ImageSource))]
	public class MsgTypeToResourceConverter : IValueConverter
	{
		#region IValueConverter Members
		/// <summary> 
		/// Converts a value. 
		/// </summary> 
		/// <param name="value">The value produced by the binding source.</param> 
		/// <param name="targetType">The type of the binding target property.</param> 
		/// <param name="parameter">The converter parameter to use.</param> 
		/// <param name="culture">The culture to use in the converter.</param> 
		/// <returns> 
		/// A converted value. If the method returns null, the valid null value is used. 
		/// </returns> 
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// Check input parameter types
			if (value == null)
				return Binding.DoNothing;

			if (!(value is Msg.MsgCategory))
				throw new ArgumentException("Invalid argument. Expected argument: ViewModel.Base.Msg.MsgType");

			if (targetType != typeof(System.Windows.Media.ImageSource))
				throw new ArgumentException("Invalid return type. Expected return type: System.Windows.Media.ImageSource");

			string resourceUri = "DialogCloser/Resources/icon/Unknown.png";
			switch ((Msg.MsgCategory)value)
			{
				case Msg.MsgCategory.Information:
					break;
				case Msg.MsgCategory.Error:
					resourceUri = "DialogCloser/Resources/icon/Error.png";
					break;
				case Msg.MsgCategory.Warning:
					resourceUri = "DialogCloser/Resources/icon/Warning.png";
					break;
				case Msg.MsgCategory.InternalError:
					resourceUri = "DialogCloser/Resources/icon/InternalError.png";
					break;
				case Msg.MsgCategory.Unknown:
				default:
					resourceUri = "DialogCloser/Resources/icon/Unknown.png";
					break;
			}

			BitmapImage icon = new BitmapImage();
			try
			{
				icon.BeginInit();
				icon.UriSource = new Uri(string.Format("pack://application:,,,/{0};component/{1}", Assembly.GetEntryAssembly().GetName().Name, resourceUri));
				icon.EndInit();
			}
			catch
			{
				return Binding.DoNothing;
			}

			return icon;
		}

		/// <summary> 
		/// Converts a value. 
		/// </summary> 
		/// <param name="value">The value that is produced by the binding target.</param> 
		/// <param name="targetType">The type to convert to.</param> 
		/// <param name="parameter">The converter parameter to use.</param> 
		/// <param name="culture">The culture to use in the converter.</param> 
		/// <returns> 
		/// A converted value. If the method returns null, the valid null value is used. 
		/// </returns> 
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Visibility && targetType == typeof(bool))
			{
				Visibility val = (Visibility)value;
				if (val == Visibility.Visible)
					return true;
				else
					return false;
			}

			throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
		}
		#endregion
	}
}
