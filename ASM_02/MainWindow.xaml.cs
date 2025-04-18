using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using Timer = System.Timers.Timer;
using System.Timers;

namespace ASM_02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SyncTimeClass stc = new SyncTimeClass();
        Binding myBinding = new Binding("Time");
        MyTextBlock lastMyTextBlockClicked;
        //DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Timer timer = new Timer(100);
        int tenthsOfSecondElapsed;        
        int pairsFound;
        int numberOfPairs;
        Random random = new Random();
        string[] allPics = {"🐗","🐨","🦌","🐸","🐪","🐩","🐏","🐐","🦘","🐫",
                            "🐌","🦨","🐿️","🦫","🦕","🦦","🐟","🐠","🦑","🦞",
                            "🦀","🦆","🐓","🐦‍","🦣","🦤","🦩","🦋","🐣","🐱",
                            "🐶","🐵","🐯","🐺","🦁","🦒","🦊","🦝","🐮","🐷",
                            "🐭","🐰","🐻","🐼","🦓","🦄","🐔","🦍","🦛","🦏",
                            "🐘","🦔","🦎","🐊","🐢","🐍","🐉","🦖","🦈","🐬",
                            "🦭","🐋","🦐","🦢","🕊️","🦜","🦉","🐧","🐤","🦇",
                            "🐝","🐎"};
        bool[,] grid, gridCopy;
        int countDownTime;
        List<MyTextBlock> lastMyTextBlocksList = new List<MyTextBlock>();
        int lastMyTextBlocksListSize;
        Stopwatch stopwatch;
        int clicksNumber;
        int combosNumber;
        int score;

        public MainWindow()
        {
            InitializeComponent();            

            menuItemScore.IsChecked = true;

            //dispatcherTimer.Interval = TimeSpan.FromSeconds(.1);
            //dispatcherTimer.Tick += DispatcherTimer_Tick;

            //timer.Elapsed += DispatcherTimer_Tick;
            timer.AutoReset = true;

            myBinding.Source = stc;
            myBinding.Mode = BindingMode.OneWay;

            tblTime.SetBinding(TextBlock.TextProperty, myBinding);
        }

        private void CreateGrid(int numberOfRows, int numberOfColumns)
        {
            grdGame.Visibility = Visibility.Visible;
            grdGame.Children.Clear();
            grdGame.RowDefinitions.Clear();
            grdGame.ColumnDefinitions.Clear();

            grdGame.Width = numberOfColumns * 80;
            grdGame.Height = numberOfRows * 80;            

            for (int i = 0; i < numberOfRows; i++)
            {
                grdGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < numberOfColumns; i++)
            {
                grdGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        private void CreateElements(int numberOfRows, int numberOfColumns)
        {
            grid = new bool[numberOfRows, numberOfColumns];
            gridCopy = new bool[numberOfRows, numberOfColumns];
            
            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    Border border = new Border();                    
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    MyTextBlock myTextBlock = new MyTextBlock(i,j);
                    myTextBlock.Width = 80;
                    myTextBlock.Height = 80;
                    myTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    myTextBlock.VerticalAlignment = VerticalAlignment.Center;
                    myTextBlock.Background = new SolidColorBrush(Color.FromArgb(255,237,237,237));                    
                    Grid.SetRow(myTextBlock, i);
                    Grid.SetColumn(myTextBlock, j);                                        

                    grdGame.Children.Add(myTextBlock);
                    grdGame.Children.Add(border);

                    grid[i, j] = true;
                    gridCopy[i, j] = true;
                }
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //tblTime.Text = ((++tenthsOfSecondElapsed) / 10F).ToString("0.0 s");
            stc.Time = ((++tenthsOfSecondElapsed) / 10F).ToString("0.0 s");            
        }

        private void DispatcherTimer_Tick2(object sender, EventArgs e)
        {
            //tblTime.Text = ((++tenthsOfSecondElapsed) / 10F).ToString("0.0 s");
            stc.Time = ((++tenthsOfSecondElapsed) / 10F).ToString("0.0 s");

            if (tenthsOfSecondElapsed == countDownTime * 10)
            {
                menuItemPause.Dispatcher.Invoke(new Action(() =>
                {
                    EndGame();
                    MessageBox.Show("Time is out!");
                }));
            }
        }

        private void AssignPics(int numberOfRows, int numberOfColumns)
        {
            numberOfPairs = numberOfRows * numberOfColumns / 2;

            string[] pics = new string[numberOfPairs];

            for(int i = 0; i < pics.Length; i++)
            {
                int a = allPics.Length - i;
                int x = random.Next(a);

                pics[i] = allPics[x];

                allPics[x] = allPics[a - 1];
                allPics[a - 1] = pics[i];
            }
                        
            pics = pics.Concat(pics).ToArray();

            //Way 1: Assigning a random cell to each pic
            //for(int i = 0; i < pics.Length; i++)
            //{
            //    int x = random.Next(grdGame.Children.Count - i);

            //    TextBlock myTextBlock = (TextBlock)grdGame.Children[x];
            //    myTextBlock.Text = pics[i];                

            //    //Rearranging the collection so the unassigned childs (TextBlocks) will be at first positions in the collection.
            //    grdGame.Children.RemoveAt(x);
            //    grdGame.Children.Add(myTextBlock);                
            //}

            //Way 2: Assigning a random pic to each cell using LINQ
            //for (int i = 0; i < grdGame.Children.Count; i++)
            //{
            //    int x = random.Next(pics.Length);

            //    ((TextBlock)grdGame.Children[i]).Text = pics[x];

            //    //Marking the already assigned pic somehow (here it is marked by assinging null)
            //    pics[x] = null;
            //    //Substituting a newly created array without the already assigned pic for the old array with that pic.
            //    pics = pics.Where((str, index) => { return pics[index] != null; }).ToArray();
            //}

            //Way 3: Assigning a random pic to each cell            

            MyTextBlock[] myTextBlocks = grdGame.Children.OfType<MyTextBlock>().ToArray();
            
            for (int i = 0; i < myTextBlocks.Length; i++)
            {
                int a = pics.Length - i;
                int x = random.Next(a);

                myTextBlocks[i].picture = pics[x];
                                    
                //Rearranging the array so the unassigned pics will be at first positions in the array.
                pics[x] = pics[a - 1];
            }

            if (menuItemVisibility.IsChecked)
            {
                SetUpForPicsShown(myTextBlocks);
            }
            else
            {
                SetUpForNumbersShown(myTextBlocks);
            }
        }

        private void SetUpForPicsShown(IEnumerable<MyTextBlock> myTextBlocks)
        {
            foreach (MyTextBlock myTextBlock in myTextBlocks)
            {
                myTextBlock.MouseLeftButtonUp += TextBlockPicsShown_MouseLeftButtonUp;
                myTextBlock.ShowPicture();
            }

            tblClicksNumber.Visibility = Visibility.Collapsed;
            tblCombosNumber.Visibility = Visibility.Collapsed;
            tblScore.Visibility = Visibility.Collapsed;
        }

        private void SetUpForNumbersShown(IEnumerable<MyTextBlock> myTextBlocks)
        {
            foreach (MyTextBlock myTextBlock in myTextBlocks)
            {
                myTextBlock.MouseLeftButtonUp += TextBlockNumbersShown_MouseLeftButtonUp;
                myTextBlock.ShowNumber();
            }

            stopwatch = new Stopwatch();

            clicksNumber = 0;
            combosNumber = 0;
            score = 0;

            tblClicksNumber.Visibility = Visibility.Visible;
            tblCombosNumber.Visibility = Visibility.Visible;
            tblScore.Visibility = Visibility.Visible;

            tblClicksNumber.Text = "Number of clicks: " + clicksNumber;
            tblCombosNumber.Text = "Number of combos: " + combosNumber;
            tblScore.Text = "Score: " + score;
        }

        private void MenuItemNewGame_Click(object sender, RoutedEventArgs e)
        {            
            int numberOfRows = int.Parse(tbRows.Text);
            int numberOfColumns = int.Parse(tbColumns.Text);

            if (numberOfRows * numberOfColumns % 2 != 0)
            {
                MessageBox.Show("Total number of elements should be even.");
                return;
            }

            SetSettignsUp(numberOfRows, numberOfColumns);

            CreateGrid(numberOfRows, numberOfColumns);

            CreateElements(numberOfRows, numberOfColumns);
            
            AssignPics(numberOfRows, numberOfColumns);   
            
            menuItemNewGame.IsEnabled = false;

            Thread.Sleep(500);

            //dispatcherTimer.Start();
            timer.Start();

            new Thread(() =>
            {
                Thread.Sleep(5000);
                menuItemNewGame.Dispatcher.Invoke(new Action(() =>
                {
                    menuItemNewGame.IsEnabled = true;
                }));                
            }).Start();
        }

        private void SetSettignsUp(int numberOfRows, int numberOfColumns)
        {
            menuItemVisibility.IsEnabled = false;

            menuItemSetTimer.IsEnabled = false;

            if (menuItemSetTimer.IsChecked)
            {
                timer.Elapsed -= DispatcherTimer_Tick;
                timer.Elapsed -= DispatcherTimer_Tick2;
                timer.Elapsed += DispatcherTimer_Tick2;

                if (!int.TryParse(tbSetTime.Text, out countDownTime)) countDownTime = 15;
            }
            else
            {
                timer.Elapsed -= DispatcherTimer_Tick;
                timer.Elapsed -= DispatcherTimer_Tick2;
                timer.Elapsed += DispatcherTimer_Tick;
            }

            menuItemPause.Visibility = Visibility.Visible;
            menuItemPause.Header = "Pause";
            grdGame.IsEnabled = true;
            tenthsOfSecondElapsed = 0;
            pairsFound = 0;
            tblPairsNumber.Text = "Number of pairs: " + pairsFound;            
            lastMyTextBlocksList.Clear();
            lastMyTextBlocksListSize = numberOfRows * numberOfColumns / 12;
        }

        private void TextBlockPicsShown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyTextBlock myTextBlock = (MyTextBlock)sender;

            if (lastMyTextBlockClicked != null)
            {
                if (lastMyTextBlockClicked.picture.Equals(myTextBlock.picture))
                {
                    int r1 = Grid.GetRow(lastMyTextBlockClicked);
                    int c1 = Grid.GetColumn(lastMyTextBlockClicked);
                    int r2 = Grid.GetRow(myTextBlock);
                    int c2 = Grid.GetColumn(myTextBlock);

                    if (Utility.IsReachable(gridCopy, r1, c1, r2, c2))
                    {
                        grid[r1, c1] = false;
                        grid[r2, c2] = false;
                        gridCopy = (bool[,])grid.Clone();

                        grdGame.Children.Remove(myTextBlock);
                        grdGame.Children.Remove(lastMyTextBlockClicked);

                        pairsFound++;

                        tblPairsNumber.Text = "Number of pairs: " + pairsFound;

                        if (pairsFound == numberOfPairs)
                        {
                            EndGame();
                            MessageBox.Show("Congratulations! You matched all pics.");
                        }                        
                    }
                    else
                    {
                        lastMyTextBlockClicked.Visibility = Visibility.Visible;                        
                    }
                }
                else
                {
                    lastMyTextBlockClicked.Visibility = Visibility.Visible;
                }

                lastMyTextBlockClicked = null;
            }
            else
            {
                lastMyTextBlockClicked = myTextBlock;
                lastMyTextBlockClicked.Visibility = Visibility.Hidden;                
            }
        }

        private void TextBlockNumbersShown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {               
            MyTextBlock myTextBlock = (MyTextBlock)sender;
            myTextBlock.Foreground = Brushes.DimGray;
            myTextBlock.Background = new SolidColorBrush(Color.FromArgb(255, 247, 247, 247));
            tblClicksNumber.Text = "Number of clicks: " + (++clicksNumber);
            score--;
            tblScore.Text = "Score: " + score;

            if (lastMyTextBlockClicked != null)
            {
                lastMyTextBlockClicked.Foreground = Brushes.Black;
                lastMyTextBlockClicked.Background = new SolidColorBrush(Color.FromArgb(255, 237, 237, 237));

                if (lastMyTextBlockClicked.picture.Equals(myTextBlock.picture))
                {
                    int r1 = Grid.GetRow(lastMyTextBlockClicked);
                    int c1 = Grid.GetColumn(lastMyTextBlockClicked);
                    int r2 = Grid.GetRow(myTextBlock);
                    int c2 = Grid.GetColumn(myTextBlock);

                    if (Utility.IsReachable(gridCopy, r1, c1, r2, c2))
                    {
                        grid[r1, c1] = false;
                        grid[r2, c2] = false;
                        gridCopy = (bool[,])grid.Clone();

                        grdGame.Children.Remove(myTextBlock);
                        grdGame.Children.Remove(lastMyTextBlockClicked);

                        lastMyTextBlocksList.Remove(myTextBlock);
                        lastMyTextBlocksList.Remove(lastMyTextBlockClicked);

                        lastMyTextBlockClicked = null;

                        pairsFound++;

                        tblPairsNumber.Text = "Number of pairs: " + pairsFound;
                        score += grid.Length / 5;
                        tblScore.Text = "Score: " + score;

                        if (stopwatch.IsRunning && stopwatch.Elapsed.TotalSeconds <= 4)
                        {
                            IEnumerable<MyTextBlock> tbxsLeft = grdGame.Children.OfType<MyTextBlock>();

                            tblCombosNumber.Text = "Number of combos: " + (++combosNumber);                            
                            score += tbxsLeft.Count();
                            tblScore.Text = "Score: " + score;

                            var myTextBlocks = tbxsLeft.Where((el) =>
                            {
                                return int.TryParse(el.Text, out int n);
                            }).ToArray();
                            
                            try
                            {
                                for(int i = 0; i < 2; i++)
                                {
                                    int a = myTextBlocks.Length - i;
                                    int x = random.Next(a);
                                    myTextBlocks[x].ShowPicture();
                                    myTextBlocks[x] = myTextBlocks[a - 1];
                                }
                            }
                            catch { }                            
                        }
                        
                        stopwatch.Restart();

                        if (pairsFound == numberOfPairs)
                        {
                            EndGame();
                            MessageBox.Show("Congratulations 🤗! You matched all pics 👍");
                        }                        
                    }
                    else
                    {
                        Method1(myTextBlock);
                    }
                }
                else
                {
                    Method1(myTextBlock);
                }                
            }
            else
            {                
                lastMyTextBlockClicked = myTextBlock;
                lastMyTextBlockClicked.ShowPicture();
                lastMyTextBlockClicked.IsEnabled = false;                
            }
        }

        private void Method1(MyTextBlock myTextBlock)
        {
            if (pairsFound != 0)
            {
                if (!lastMyTextBlocksList.Contains(lastMyTextBlockClicked))
                {
                    if (lastMyTextBlocksList.Count < pairsFound && lastMyTextBlocksList.Count < lastMyTextBlocksListSize)
                    {
                        lastMyTextBlocksList.Add(lastMyTextBlockClicked);
                        lastMyTextBlockClicked.IsEnabled = true;
                    }
                    else
                    {
                        lastMyTextBlocksList[0].ShowNumber();
                        lastMyTextBlocksList.RemoveAt(0);
                        lastMyTextBlocksList.Add(lastMyTextBlockClicked);
                        lastMyTextBlockClicked.IsEnabled = true;
                    }
                }
                else lastMyTextBlockClicked.IsEnabled = true;
            }
            else
            {
                lastMyTextBlockClicked.ShowNumber();
                lastMyTextBlockClicked.IsEnabled = true;
            }

            lastMyTextBlockClicked = myTextBlock;
            lastMyTextBlockClicked.ShowPicture();
            lastMyTextBlockClicked.IsEnabled = false;
        }

        private void tbRows_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(int.TryParse(tbRows.Text, out int num) && num > 3 && num < 9))
            {
                MessageBox.Show("Restriction: 3 < rows < 9");
                tbRows.Text = "4";
            }            
        }

        private void tbColumns_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(int.TryParse(tbColumns.Text, out int num) && num > 3 && num < 17))
            {
                MessageBox.Show("Restriction: 3 < columns < 17");
                tbColumns.Text = "4";
            }
        }        

        private void menuItemPause_Click(object sender, RoutedEventArgs e)
        {
            if (menuItemPause.Header.Equals("Pause"))
            {
                grdGame.Visibility = Visibility.Hidden;
                //dispatcherTimer.Stop();
                timer.Stop();
                menuItemPause.Header = "Resume";
                try
                {
                    stopwatch.Stop();
                }
                catch { }
            }
            else
            {
                grdGame.Visibility = Visibility.Visible;
                //dispatcherTimer.Start();
                timer.Start();
                menuItemPause.Header = "Pause";
                try
                {
                    stopwatch.Start();
                }
                catch { }
            }
        }        

        private void menuItemScore_Checked(object sender, RoutedEventArgs e)
        {            
            if (menuItemVisibility.IsChecked)
            {
                tblPairsNumber.Visibility = Visibility.Visible;
            }
            else
            {
                tblClicksNumber.Visibility = Visibility.Visible;
                tblCombosNumber.Visibility = Visibility.Visible;
                tblPairsNumber.Visibility = Visibility.Visible;
                tblScore.Visibility = Visibility.Visible;
            }
        }

        private void menuItemScore_Unchecked(object sender, RoutedEventArgs e)
        {
            if (menuItemVisibility.IsChecked)
            {
                tblPairsNumber.Visibility = Visibility.Collapsed;
            }
            else
            {
                tblClicksNumber.Visibility = Visibility.Collapsed;
                tblCombosNumber.Visibility = Visibility.Collapsed;
                tblPairsNumber.Visibility = Visibility.Collapsed;
                tblScore.Visibility = Visibility.Collapsed;
            }
        }

        private void menuItemSetTimer_Checked(object sender, RoutedEventArgs e)
        {
            tblSetTime.Visibility = Visibility.Visible;
            tbSetTime.Visibility = Visibility.Visible;
        }

        private void menuItemSetTimer_Unchecked(object sender, RoutedEventArgs e)
        {
            tblSetTime.Visibility = Visibility.Hidden;
            tbSetTime.Visibility = Visibility.Hidden;
        }

        private void MenuItemEndGame_Click(object sender, RoutedEventArgs e)
        {
            EndGame();
        }        

        private void EndGame()
        {
            //dispatcherTimer.Stop();
            timer.Stop();            
            grdGame.IsEnabled = false;
            menuItemPause.Visibility = Visibility.Collapsed;
            menuItemSetTimer.IsEnabled = true;
            menuItemVisibility.IsEnabled = true;

            try
            {
                stopwatch.Reset();
            }
            catch { }            
        }
    }
}
