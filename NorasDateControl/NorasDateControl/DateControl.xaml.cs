using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NorasDateControl
{
    /// <summary>
    /// DateControl.xaml 的交互逻辑
    /// </summary>
    public partial class DateControl : UserControl
    {
        DateVM SelectedDate;
        bool isInitial=true;//是否为初始化操作

        public DateControl()
        {
            InitializeComponent();
            InitialDate();
        }
        #region ViewModel
        public class BaseViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void RaisePropertyChanged(string propertyName)
            {
                if (propertyName != null && PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class DateVM:BaseViewModel
        {
            private string year;

            public string Year
            {
                get { return year; }
                set
                {
                    year = value;
                    RaisePropertyChanged("Year");
                }
            }
            private string month;

            public string Month
            {
                get { return month; }
                set
                {
                    month = value;
                    RaisePropertyChanged("Month");
                }
            }
            private string day;

            public string Day
            {
                get { return day; }
                set
                {
                    day = value;
                    RaisePropertyChanged("Day");
                }
            }


        }
        #endregion
        #region Methods
        void InitialDate()
        {
            if (SelectedDate == null)
            {
                DateTime dt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                SelectedDate = new DateVM()
                {
                    Year = dt.Year.ToString().PadLeft(4, '0'),
                    Month = dt.Month.ToString().PadLeft(2, '0'),
                    Day = dt.Day.ToString().PadLeft(2, '0')
                };
            }

            this.DataContext = SelectedDate;
        }
        int GetDays(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }
        void UpYear()
        {
            if (!txt_Year.IsKeyboardFocused) Keyboard.Focus(txt_Year);
            int.TryParse(txt_Year.Text, out int tempYear);
            --tempYear;
            if (tempYear <= 0) tempYear = 1;//从0001开始
            txt_Year.Text = tempYear.ToString().PadLeft(4, '0');
            txt_Year.SelectionStart = txt_Year.Text.Length;
        }
        void UpMonth()
        {
            if (!txt_Month.IsKeyboardFocused) Keyboard.Focus(txt_Month);
            int.TryParse(txt_Month.Text, out int tempMonth);
            --tempMonth;
            if (tempMonth <= 0) tempMonth = 12;
            txt_Month.Text = tempMonth.ToString().PadLeft(2, '0');
            txt_Month.SelectionStart = txt_Month.Text.Length;
            ResetDayToFirstDay();
        }
        void UpDay()
        {
            if (!txt_Day.IsKeyboardFocused) Keyboard.Focus(txt_Day);
            int.TryParse(txt_Day.Text, out int tempDay);
            --tempDay;
            if (tempDay <= 0) tempDay = GetDays(Convert.ToInt32(txt_Year.Text), Convert.ToInt32(txt_Month.Text));
            txt_Day.Text = tempDay.ToString().PadLeft(2, '0');
            txt_Day.SelectionStart = txt_Day.Text.Length;
        }
        void DownYear()
        {
            if (!txt_Year.IsKeyboardFocused) Keyboard.Focus(txt_Year);
            int.TryParse(txt_Year.Text, out int tempYear);
            ++tempYear;
            txt_Year.Text = tempYear.ToString().PadLeft(4, '0');
            txt_Year.SelectionStart = txt_Year.Text.Length;
        }
        void DownMonth()
        {
            if (!txt_Month.IsKeyboardFocused) Keyboard.Focus(txt_Month);
            int.TryParse(txt_Month.Text, out int tempMonth);
            ++tempMonth;
            if (tempMonth > 12) tempMonth = 1;
            txt_Month.Text = tempMonth.ToString().PadLeft(2, '0');
            txt_Month.SelectionStart = txt_Month.Text.Length;
            ResetDayToFirstDay();
        }
        void DownDay()
        {
            if (!txt_Day.IsKeyboardFocused) Keyboard.Focus(txt_Day);
            int.TryParse(txt_Day.Text, out int tempDay);
            ++tempDay;
            if (tempDay > GetDays(Convert.ToInt32(txt_Year.Text), Convert.ToInt32(txt_Month.Text))) tempDay = 1;
            txt_Day.Text = tempDay.ToString().PadLeft(2, '0');
            txt_Day.SelectionStart = txt_Day.Text.Length;
        }
        bool IsMatch(string pattern, string content)
        {
            return new Regex(pattern).IsMatch(content);
        }
        void LimitNoSpaceForTextBox(TextBox textBox)
        {
            bool isMatch = IsMatch(@"^[\s]*", textBox.Text);
            if (isMatch)
            {
                textBox.Text = textBox.Text.Trim();
            }
        }
        void ResetDayToFirstDay()
        {
            int.TryParse(txt_Year.Text, out int tempYear);
            int.TryParse(txt_Month.Text, out int tempMonth);
            if (tempYear > 0 && (tempMonth >= 1 && tempMonth <= 12))
            {
                int.TryParse(txt_Day.Text, out int tempDay);
                if (tempDay > GetDays(tempYear, tempMonth))
                {
                    tempDay = 1;
                    txt_Day.Text = tempDay.ToString().PadLeft(2, '0');
                }
            }
        }
        //获取视觉树上的控件
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        void AppendYear()
        {
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(lv_Year);
            //获得滚动条是否显示
            Visibility verticalVisibility = scrollViewer.ComputedVerticalScrollBarVisibility;
            Visibility horizontalVisibility = scrollViewer.ComputedHorizontalScrollBarVisibility;
            //如果VerticalOffset和ScrollableHeight相等表明已经到最底了
            if (verticalVisibility==Visibility.Visible&&scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                ListViewItem lastItem = lv_Year.Items[lv_Year.Items.Count - 1] as ListViewItem;
                for (int i = 1; i <= 10; i++)
                {
                    ListViewItem afterItem = new ListViewItem() { Content = Convert.ToInt32(lastItem.Content) + i };
                    lv_Year.Items.Add(afterItem);
                }
                lv_Year.ScrollIntoView(lastItem);
            }
            //非初始化操作 且 到最顶端
            if (verticalVisibility == Visibility.Visible&&!isInitial && scrollViewer.VerticalOffset == 0)
            {
                ListViewItem firstItem = lv_Year.Items[0] as ListViewItem;
                for (int i = 1; i <= 10; i++)
                {
                    int beforeYear = Convert.ToInt32(firstItem.Content) - i;
                    if (beforeYear > 0)
                    {
                        ListViewItem beforeItem = new ListViewItem() { Content = beforeYear.ToString().PadLeft(4,'0') };
                        lv_Year.Items.Insert(0, beforeItem);
                    }
                }
                lv_Year.ScrollIntoView(firstItem);
            }
        }
        void InitialPopupYear()
        {
            pop_Year.IsOpen = true;

            lv_Year.Items.Clear();
            int currentYear;
            if (!string.IsNullOrEmpty(txt_Year.Text.Trim()))
            {
                currentYear = Convert.ToInt32(txt_Year.Text);
            }
            else currentYear = DateTime.Now.Year;
            for (int i = 10; i >= 1; i--)
            {
                int beforeYear = currentYear - i;
                if (beforeYear > 0)
                {
                    ListViewItem beforeItem = new ListViewItem() { Content = beforeYear.ToString().PadLeft(4, '0') };
                    lv_Year.Items.Add(beforeItem);
                }
            }
            ListViewItem currentItem = new ListViewItem() { Content = currentYear, IsSelected = true };
            lv_Year.Items.Add(currentItem);
            for (int i = 1; i <= 10; i++)
            {
                ListViewItem afterItem = new ListViewItem() { Content = Convert.ToInt32(txt_Year.Text) + i };
                lv_Year.Items.Add(afterItem);
            }
            lv_Year.ScrollIntoView(currentItem);
        }
        void InitialPopupMonth()
        {
            pop_Month.IsOpen = true;
            if (lv_Month.Items.Count == 0)
            {
                int currentMonth;
                if (!string.IsNullOrEmpty(txt_Month.Text.Trim()))
                {
                    currentMonth = Convert.ToInt32(txt_Month.Text.Trim());
                }
                else currentMonth = DateTime.Now.Month;
                ListViewItem currentItem = null;
                for (int i = 1; i <= 12; i++)
                {
                    ListViewItem monthItem = new ListViewItem() { Content = i.ToString().PadLeft(2, '0') };
                    if (i == currentMonth)
                    {
                        monthItem.IsSelected = true;
                        currentItem = monthItem;
                    }
                    lv_Month.Items.Add(monthItem);
                }
                lv_Month.ScrollIntoView(currentItem);
            }
        }
        void InitialPopupDay()
        {
            if (!string.IsNullOrEmpty(txt_Year.Text.Trim()) && !string.IsNullOrEmpty(txt_Month.Text.Trim()))
            {
                pop_Day.IsOpen = true;
                lv_Day.Items.Clear();

                int days = GetDays(Convert.ToInt32(txt_Year.Text.Trim()), Convert.ToInt32(txt_Month.Text.Trim()));
                ListViewItem currentItem = null;
                DateTime dt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
                int currentDay = dt.Day;
                for (int i = 1; i <= days; i++)
                {
                    ListViewItem dayItem = new ListViewItem() { Content = i.ToString().PadLeft(2, '0') };
                    if (i == currentDay)
                    {
                        dayItem.IsSelected = true;
                        currentItem = dayItem;
                    }
                    lv_Day.Items.Add(dayItem);
                }
                lv_Day.ScrollIntoView(currentItem);
            }
            else MessageBox.Show("年度或月份均不能为空！");
        }
        public DateTime GetDate()
        {
           return new DateTime(Convert.ToInt32(SelectedDate.Year), Convert.ToInt32(SelectedDate.Month),
                                     Convert.ToInt32(SelectedDate.Day));
        }
        public void SetDate(DateTime date)
        {
            txt_Year.Text = date.Year.ToString().PadLeft(4, '0');
            txt_Month.Text = date.Month.ToString().PadLeft(2, '0');
            txt_Day.Text = date.Day.ToString().PadLeft(2, '0');
        }
        #endregion

        private void Txt_Year_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && txt_Year.SelectionStart == txt_Year.Text.Length)
            {
                Keyboard.Focus(txt_Month);
                txt_Month.SelectionStart = txt_Month.Text.Length;
            }
            if (e.Key == Key.Up)
            {
                UpYear();
            }
            if (e.Key == Key.Down)
            {
                DownYear();
            }
        }

        private void Txt_Month_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && txt_Month.SelectionStart == txt_Month.Text.Length)
            {
                Keyboard.Focus(txt_Day);
                txt_Day.SelectionStart = txt_Day.Text.Length;
            }
            if (e.Key == Key.Left && txt_Month.SelectionStart == 0)
            {
                Keyboard.Focus(txt_Year);
                txt_Year.SelectionStart = txt_Year.Text.Length;
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                UpMonth();
            }
            if (e.Key == Key.Down)
            {
                DownMonth();
            }
        }

        private void Txt_Day_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && txt_Day.SelectionStart == 0)
            {
                Keyboard.Focus(txt_Month);
                txt_Month.SelectionStart = txt_Month.Text.Length;
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                UpDay();
            }
            if (e.Key == Key.Down)
            {
                DownDay();
            }
        }

        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string old = txtBox.Text;
            int index = txtBox.SelectionStart;
            string newStr = string.Empty;
            newStr = old.Substring(0, index) + e.Text + old.Substring(index, old.Length - index);
            if (txtBox.Name == "txt_Year")
            {
                int.TryParse(newStr, out int tempYear);
                e.Handled = !(new Regex(@"^\d{1,4}$").IsMatch(newStr) && tempYear > 0);
            }
            if (txtBox.Name == "txt_Month")
            {
                int.TryParse(newStr, out int tempMonth);
                e.Handled = !(new Regex(@"^\d{1,2}$").IsMatch(newStr) && tempMonth > 0 && tempMonth <= 12);
            }
            if (txtBox.Name == "txt_Day")
            {
                int.TryParse(newStr, out int tempDay);
                e.Handled = !(new Regex(@"^\d{1,2}$").IsMatch(newStr) && tempDay > 0 && tempDay <=
                    GetDays(Convert.ToInt32(txt_Year.Text), Convert.ToInt32(txt_Month.Text)));
            }
        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox.Text.Contains(" ")) LimitNoSpaceForTextBox(txtBox);

            if (txtBox.Name == "txt_Month")
            {
                ResetDayToFirstDay();
            }

            //if (string.IsNullOrEmpty(txtBox.Text))
            //{
            //    if (txtBox.Name == "txt_Year")
            //        txtBox.Text = "1".PadLeft(4, '0');
            //    else txtBox.Text = "1".PadLeft(2,'0');
            //    txtBox.SelectionStart = txtBox.Text.Length;
            //}
            txtBox.SelectionStart = txtBox.Text.Length;
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            int.TryParse(txtBox.Text,out int value);
            string strText = string.Empty;
            if (value == 0)
            {
                strText = "1";
            }
            else strText = txtBox.Text.Trim();
            if (txtBox.Name == "txt_Year")
            {
                txtBox.Text = strText.PadLeft(4, '0');
                pop_Year.IsOpen = false;
            }
            else
            {
                txtBox.Text = strText.PadLeft(2, '0');
                if (txtBox.Name == "txt_Month") pop_Month.IsOpen = false;
                if (txtBox.Name == "txt_Day") pop_Day.IsOpen = false;
            }
            txtBox.SelectionStart = txtBox.Text.Length;
        }

        private void Lv_Year_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isInitial) isInitial = false;
            AppendYear();
        }

        private void Lv_Year_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem selectedItem = lv_Year.SelectedItem as ListViewItem;
            if (selectedItem != null)
            {
                txt_Year.Text = selectedItem.Content.ToString();
            }
        }

        private void Lv_Year_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (lv_Year.Items.Count != 0)
            {
                AppendYear();
            }
        }

        private void Lv_Month_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem selectedItem = lv_Month.SelectedItem as ListViewItem;
            if (selectedItem != null)
            {
                txt_Month.Text = selectedItem.Content.ToString();
            }
        }

        private void Lv_Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem selectedItem = lv_Day.SelectedItem as ListViewItem;
            if (selectedItem != null)
            {
                txt_Day.Text = selectedItem.Content.ToString();
            }
        }

        private void Txt_Year_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InitialPopupYear();
        }
        private void DateControl_Loaded(object sender, RoutedEventArgs e)
        {
            isInitial = false;//标识初始化操作已完成
        }

        private void Txt_Month_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InitialPopupMonth();
        }

        private void Txt_Day_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InitialPopupDay();
        }

        private void Lv_popup_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup popup = sender as Popup;
            popup.IsOpen = false;
        }
    }
}
