using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PosModels;
using PosControls.Interfaces;
using PosControls;
using TemPOS.Types;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for TicketItemTemplate.xaml
    /// </summary>
    public partial class TicketItemTemplate : UserControl, ICloneable, ISelectable
    {
        #region ISelectable
        public bool IsSelectable
        {
            get { return true; }
            set { }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
            "IsSelectedProperty", typeof(bool), typeof(TicketItemTemplate),
            new UIPropertyMetadata(false,
                IsSelectedValueChanged, IsSelectedCoerceValue));

        protected static object IsSelectedCoerceValue(DependencyObject depObject, object value)
        {
            TicketItemTemplate myClass = (TicketItemTemplate)depObject;
            bool newValue = (bool)value;
            if (newValue && !myClass.IsSelectable)
                return false;
            return value;
        }

        protected static void IsSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TicketItemTemplate myClass = (TicketItemTemplate)d;
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;
            // A tag value of null is the same as IsSelected == true
            if (newValue)
            {
                myClass.Tag = null;
                myClass.GridControl.Background = ConfigurationManager.ListItemSelectedBackgroundBrush;
            }
            else
            {
                myClass.Tag = true;
                myClass.GridControl.Background = ConfigurationManager.ListItemBackgroundBrush;
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        #endregion

        private TicketItem _ticketItem;
        private BranchType _branching = BranchType.None;

        public TicketItem TicketItem
        {
            get { return _ticketItem; }
            set
            {
                _ticketItem = value;
                Update();
            }
        }

        public string CommentText
        {
            get
            {
                return textBoxComment.Text;
            }
            set
            {
                textBoxComment.Visibility = ((value == null) ? Visibility.Hidden : Visibility.Visible);
                textBoxComment.Text = value;
            }
        }

        public BranchType Branching
        {
            get
            {
                return _branching;
            }
            set
            {
                _branching = value;
                UpdateDropLines();
            }
        }

        private void UpdateDropLines()
        {
            if (_branching == BranchType.None)
            {
                canvasDropDown.Width = 0;
                return;
            }
            
            // Draw Branch to canvasDropDown
            canvasDropDown.Width = 40;
            canvasDropDown.Height = ActualHeight;
            canvasDropDown.Children.Clear();
            canvasDropDown.UpdateLayout();

            Line verticalLine = new Line();
            verticalLine.X1 =
                verticalLine.X2 = 20;
            verticalLine.Y1 = 0;
            if (_branching == BranchType.Middle)
                verticalLine.Y2 = ActualHeight;
            else
                verticalLine.Y2 = ActualHeight / 2;
            verticalLine.StrokeThickness = 1;
            verticalLine.Stroke = Brushes.White;

            Line horizontalLine = new Line
            {
                X1 = 20,
                X2 = 40
            };
            horizontalLine.Y1 =
                horizontalLine.Y2 = ActualHeight / 2;
            horizontalLine.StrokeThickness = 1;
            horizontalLine.Stroke = Brushes.White;

            canvasDropDown.Children.Add(verticalLine);
            canvasDropDown.Children.Add(horizontalLine);
            canvasDropDown.UpdateLayout();
        }

        public TicketItemTemplate(TicketItem tItem, BranchType branching = BranchType.None)
        {
            Tag = true;
            InitializeComponent();
            TicketItem = tItem;
            IsSelected = false;
            Branching = branching;
            GridControl.Background = ConfigurationManager.ListItemBackgroundBrush;
            Loaded += TicketItemTemplate_Loaded;
        }

        [Obfuscation(Exclude = true)]
        void TicketItemTemplate_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDropLines();
        }

        public TicketItemTemplate(TicketItem tItem, string comment, BranchType branching = BranchType.None)
            : this(tItem, branching)
        {
            CommentText = comment;
        }

        /// <summary>
        /// This method will update the control with the information contained
        /// in the current TicketItem.
        /// </summary>
        public void Update()
        {
            // Get the category
            Item item = Item.Get(_ticketItem.ItemId);
            string itemName = item.ShortName + Environment.NewLine;
            if (!String.IsNullOrEmpty(item.FullName))
                itemName = item.FullName + Environment.NewLine;

            // Get the options for this ticket item's category
            var ticketItemOptions =
                new List<TicketItemOption>(TicketItemOption.GetAll(_ticketItem.PrimaryKey));

            // Process the option sets for this ticket item
            ProcessItemOption(ticketItemOptions, item.ItemOptionSetIds[0], ref itemName);
            ProcessItemOption(ticketItemOptions, item.ItemOptionSetIds[1], ref itemName);
            ProcessItemOption(ticketItemOptions, item.ItemOptionSetIds[2], ref itemName);

            //ItemOptionsExtraCost = itemOptionsCost;
            itemName = itemName.Substring(0, itemName.Length - Environment.NewLine.Length);

            if (_ticketItem.SpecialInstructions != null)
                itemName += Environment.NewLine + _ticketItem.SpecialInstructions;

            // Set the text
            textBoxItem.Text = itemName;
            textBoxQuantity.Text = "" + ((_ticketItem.QuantityPending != null) ?
                _ticketItem.QuantityPending.Value : _ticketItem.Quantity);
            if (_ticketItem.QuantityPendingReturn > 0)
                textBoxQuantity.Text = "" + _ticketItem.QuantityPendingReturn;
            
            // Price (total with item options)
            if (_ticketItem.IsPendingReturn)
                CommentText = Strings.Return;
            else if (_ticketItem.IsCanceled)
                CommentText = Strings.Canceled;
            else if (TicketItem.ParentTicketItemId.HasValue)
                CommentText = "";
            else
                CommentText = String.Format("{0:C}",
                    _ticketItem.GetTotalCost(_ticketItem.QuantityPendingReturn));
        }

        private void ProcessItemOption(IEnumerable<TicketItemOption> ticketItemOptions,
            int setId, ref string itemName)
        {
            bool found = false;
            foreach (TicketItemOption ticketItemOption in ticketItemOptions)
            {
                foreach (ItemOption itemOption in ItemOption.GetInSet(setId))
                {
                    if (itemOption.Id == ticketItemOption.ItemOptionId)
                    {
                        if (found)
                            itemName += ", ";
                        found = true;
                        itemName += itemOption.Name;
                    }
                }
            }
            if (found)
                itemName += Environment.NewLine;
        }

        public object Clone()
        {
            return new TicketItemTemplate(TicketItem, CommentText);
        }
        
        [Obfuscation(Exclude = true)]
        private void mainPane_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //UpdateListBoxBasedOnLocalEvent();
        }
    }
}
