using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics;

namespace BuildTree.ViewModel
{
	/// <summary>
	/// Code from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
	/// 
	/// Base class for all ViewModel classes in the application.
	/// It provides support for property change notifications 
	/// and has a DisplayName property.  This class is abstract.
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
					throw new Exception(msg);
				else
					Debug.Fail(msg);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might 
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

		#endregion // Debugging Aides

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Tell bound controls (via WPF binding) to refresh their display.
		/// 
		/// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
		/// where 'this' is derived from <seealso cref="BaseViewModel"/>
		/// and IsSelected is a property.
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="property"></param>
		protected void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
		{
			var lambda = (LambdaExpression)property;
			MemberExpression memberExpression;

			var body = lambda.Body as UnaryExpression;
			if (body != null)
			{
				var unaryExpression = body;
				memberExpression = (MemberExpression)unaryExpression.Operand;
			}
			else
				memberExpression = (MemberExpression)lambda.Body;

			this.OnPropertyChanged(memberExpression.Member.Name);
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.VerifyPropertyName(propertyName);

			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}
		#endregion // INotifyPropertyChanged Members

		#region DisplayName

		/// <summary>
		/// Returns the user-friendly name of this object.
		/// Child classes can set this property to a new value,
		/// or override it to determine the value on-demand.
		/// </summary>
		public string DisplayName { get; protected set; }

		#endregion // DisplayName

		#region IDisposable implementation
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// NOTE: Leave out the finalizer altogether if this class doesn't
		// own unmanaged resources itself, but leave the other methods
		// exactly as they are.
		~ViewModelBase()
		{
			// Finalizer calls Dispose(false)
#if DEBUG
			// useful for ensuring that viewmodel objects are properly garbage collected				
			string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
			System.Diagnostics.Debug.WriteLine(msg);
#endif 
			Dispose(false);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources here

				// free unmanaged resources here
			}
		}
		#endregion
			
	}
}
