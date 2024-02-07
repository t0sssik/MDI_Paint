using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.MDI;

namespace MDIPaint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Цвет пера
        /// </summary>
        private static Color PenColor { get; set; }
        /// <summary>
        /// Размер пера
        /// </summary>
        private static double PenSize { get; set; }
        /// <summary>
        /// Коллекция всех текущих холстов
        /// </summary>
        private static ObservableCollection<InkCanvas> InkCanvases = new ObservableCollection<InkCanvas>();
        /// <summary>
        /// Текущая линия
        /// </summary>
        private Line currentLine;
        /// <summary>
        /// Текущий эллипс
        /// </summary>
        private Ellipse currentEllipse;
        /// <summary>
        /// Текущая точка
        /// </summary>
        private Point currentPoint;

        /// <summary>
        /// Иничиализация при запуске окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SaveButton.IsEnabled = false;
            SmallSaveButton.IsEnabled = false;
            SaveAsButton.IsEnabled = false;
            PenButton.IsEnabled = false;
            EraserButton.IsEnabled = false;
            LineButton.IsEnabled = false;
            EllipseButton.IsEnabled = false;
            StarButton.IsEnabled = false;
            RedColorButton.IsEnabled = false;
            BlackColorButton.IsEnabled = false;
            BlueColorButton.IsEnabled = false;
            GreenColorButton.IsEnabled = false;
            YellowColorButton.IsEnabled = false;
            PurpleColorButton.IsEnabled = false;
            OnePxSizeButton.IsEnabled = false;
            ThreePxSizeButton.IsEnabled = false;
            FivePxSizeButton.IsEnabled = false;
            EightPxSizeButton.IsEnabled = false;
            ZoomInButton.IsEnabled = false;
            ZoomOutButton.IsEnabled = false;
            StarSettings.Visibility = Visibility.Hidden;
            PenColor = Color.FromRgb(0, 0, 0);
            PenSize = 3.0;
            InkCanvases.CollectionChanged += InkCanvases_CollectionChanged;
        }

        /// <summary>
        /// Событие при изменении коллекции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Действие над коллекцией</param>
        private void InkCanvases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Событие сработает при любых изменениях в коллекции
            switch (e.Action)
            { 
                case NotifyCollectionChangedAction.Add:
                    SaveButton.IsEnabled = true;
                    SmallSaveButton.IsEnabled = true;
                    SaveAsButton.IsEnabled = true;

                    PenButton.IsEnabled = true;
                    EraserButton.IsEnabled = true;
                    LineButton.IsEnabled = true;
                    EllipseButton.IsEnabled = true;
                    StarButton.IsEnabled = true;

                    RedColorButton.IsEnabled = true;
                    BlackColorButton.IsEnabled = true;
                    BlueColorButton.IsEnabled = true;
                    GreenColorButton.IsEnabled = true;
                    YellowColorButton.IsEnabled = true;
                    PurpleColorButton.IsEnabled = true;

                    OnePxSizeButton.IsEnabled = true;
                    ThreePxSizeButton.IsEnabled = true;
                    FivePxSizeButton.IsEnabled = true;
                    EightPxSizeButton.IsEnabled = true;

                    PenButton.IsChecked = true;
                    ThreePxSizeButton.IsChecked = true;
                    BlackColorButton.IsChecked = true;

                    ZoomInButton.IsEnabled = true;
                    ZoomOutButton.IsEnabled = true;

                    break; 
                case NotifyCollectionChangedAction.Remove:
                    if (InkCanvases.Count == 0)
                    {
                        SaveButton.IsEnabled = false;
                        SmallSaveButton.IsEnabled = false;
                        SaveAsButton.IsEnabled = false;
                        PenButton.IsEnabled = false;
                        EraserButton.IsEnabled = false;
                        LineButton.IsEnabled = false;
                        EllipseButton.IsEnabled = false;
                        StarButton.IsEnabled = false;
                        RedColorButton.IsEnabled = false;
                        BlackColorButton.IsEnabled = false;
                        BlueColorButton.IsEnabled = false;
                        GreenColorButton.IsEnabled = false;
                        YellowColorButton.IsEnabled = false;
                        PurpleColorButton.IsEnabled = false;
                        OnePxSizeButton.IsEnabled = false;
                        ThreePxSizeButton.IsEnabled = false;
                        FivePxSizeButton.IsEnabled = false;
                        EightPxSizeButton.IsEnabled = false;
                        ZoomInButton.IsEnabled = false;
                        ZoomOutButton.IsEnabled = false;
                        StarSettings.Visibility = Visibility.Hidden;
                    }
                    break;
            }
        }
        /// <summary>
        /// Событие при изменении элемента внутри коллекции
        /// </summary>
        /// <param name="sender">Изменяемый холст</param>
        /// <param name="e"></param>
        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            ((InkCanvas)sender).Tag = false; // Устанавливаем флаг изменений
        }

        /// <summary>
        /// Событие при нажатии на кнопку Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainForm.Close();
        }
        /// <summary>
        /// Событие при нажатии на кнопку О программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }
        /// <summary>
        /// Событие при нажатии на кнопку Создать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var inkCanvas = new InkCanvas
            {
                Background = Brushes.White,
                ClipToBounds = true,
                DefaultDrawingAttributes = new DrawingAttributes
                {
                    Color = PenColor,
                    Width = PenSize,
                    Height = PenSize,
                }
            }; ;
            ScaleTransform scale = new ScaleTransform();
            inkCanvas.LayoutTransform = scale;
            inkCanvas.StrokeCollected += InkCanvas_StrokeCollected;
            inkCanvas.Tag = false;
            InkCanvases.Add(inkCanvas);

            var NewMDIChild = new WPF.MDI.MdiChild()
            {
                Title = "Новый холст",
                Cursor = Cursors.Pen,
                Content = inkCanvas
            };
            NewMDIChild.Closing += (s, args) =>
            {
                if ((bool)inkCanvas.Tag == false)
                {
                    MessageBoxResult result = MessageBox.Show("Изменения не сохранены. Хотите сохранить изменения перед закрытием?", "Предупреждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (NewMDIChild.Tag == null)
                        {
                            var saveFileDialog = new SaveFileDialog
                            {
                                Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png",
                                DefaultExt = "png"
                            };

                            if (saveFileDialog.ShowDialog() == true)
                            {
                                SaveInkCanvas(inkCanvas, saveFileDialog.FileName);
                                // Сохраняем путь к файлу в свойствах MdiChild
                                NewMDIChild.Tag = saveFileDialog.FileName;
                                NewMDIChild.Tag = true;
                            }
                        }
                        else
                        {
                            // Если это не первое сохранение, используем сохраненный путь к файлу
                            SaveInkCanvas(inkCanvas, (string)NewMDIChild.Tag);
                            inkCanvas.Tag = true;
                        }
                    }
                }
                InkCanvases.Remove(inkCanvas);
                if (InkCanvases.Count == 0)
                {
                    SaveButton.IsEnabled = false;
                    SaveAsButton.IsEnabled = false;
                    SmallSaveButton.IsEnabled = false;
                }
            };
            MdiContainer.Children.Add(NewMDIChild);
            if (InkCanvases.Count == 0)
            {
                SaveButton.IsEnabled = false;
                SaveAsButton.IsEnabled = false;
                SmallSaveButton.IsEnabled = false;
            }
            else
            {
                SaveButton.IsEnabled = true;
                SaveAsButton.IsEnabled = true;
                SmallSaveButton.IsEnabled = true;
            }
        }
        #region Изменение цвета пера
        private void RedColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(255, 0, 0);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }

        private void BlueColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(0, 56, 255);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }

        private void GreenColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(43, 214, 0);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }

        private void BlackColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(0, 0, 0);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }

        private void YellowColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(255, 199, 0);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }

        private void PurpleColorButton_Checked(object sender, RoutedEventArgs e)
        {
            PenColor = Color.FromRgb(250, 0, 255);

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Color = PenColor;
            }
        }
        #endregion

        #region Изменение размера пера
        private void OnePxSizeButton_Checked(object sender, RoutedEventArgs e)
        {
            PenSize = 1.0;

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Width = PenSize;
                canvas.DefaultDrawingAttributes.Height = PenSize;
            }
        }

        private void ThreePxSizeButton_Checked(object sender, RoutedEventArgs e)
        {
            PenSize = 3.0;

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Width = PenSize;
                canvas.DefaultDrawingAttributes.Height = PenSize;
            }
        }

        private void FivePxSizeButton_Checked(object sender, RoutedEventArgs e)
        {
            PenSize = 5.0;

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Width = PenSize;
                canvas.DefaultDrawingAttributes.Height = PenSize;
            }
        }

        private void EightPxSizeButton_Checked(object sender, RoutedEventArgs e)
        {
            PenSize = 8.0;

            foreach (var canvas in InkCanvases)
            {
                canvas.DefaultDrawingAttributes.Width = PenSize;
                canvas.DefaultDrawingAttributes.Height = PenSize;
            }
        }
        #endregion

        #region Сохранение
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var activeMdiChild = MdiContainer.ActiveMdiChild;
            if (activeMdiChild == null) return;
            var activeInkCanvas = activeMdiChild.Content as InkCanvas;
            if (activeMdiChild.Tag == null)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png",
                    DefaultExt = "png"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveInkCanvas(activeInkCanvas, saveFileDialog.FileName);
                    // Сохраняем путь к файлу в свойствах MdiChild
                    activeMdiChild.Tag = saveFileDialog.FileName;
                    activeInkCanvas.Tag = true;
                }
            }
            else
            {
                // Если это не первое сохранение, используем сохраненный путь к файлу
                SaveInkCanvas(activeInkCanvas, (string)activeMdiChild.Tag);
                activeInkCanvas.Tag = true;
            }

        }
        private void SaveInkCanvas(InkCanvas inkCanvas, string filename)
        {
            // Создаем рендер и сохраняем InkCanvas в файл
            var rtb = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            rtb.Render(inkCanvas);
            var encoder = GetEncoder(filename);
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = File.OpenWrite(filename))
            {
                encoder.Save(fs);
            }
        }

        private BitmapEncoder GetEncoder(string filename)
        {
            var extension = System.IO.Path.GetExtension(filename).ToLower();
            switch (extension)
            {
                case ".bmp": return new BmpBitmapEncoder();
                case ".jpg": return new JpegBitmapEncoder();
                case ".png": return new PngBitmapEncoder();
                default: throw new InvalidOperationException("Неподдерживаемый формат файла");
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            var activeMdiChild = MdiContainer.ActiveMdiChild;
            if (activeMdiChild == null) return;
            var activeInkCanvas = activeMdiChild.Content as InkCanvas;
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png",
                FileName = "Новый рисунок"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveInkCanvas(activeInkCanvas, saveFileDialog.FileName);
                // Сохраняем путь к файлу в свойствах MdiChild
                activeMdiChild.Tag = saveFileDialog.FileName;
            }
        }
        #endregion
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            CreateButton_Click(sender, e);
            // Получаем InkCanvas из активного MdiChild
            var inkCanvas = InkCanvases[InkCanvases.Count - 1];
            if (inkCanvas == null) return;

            // Создаем и показываем OpenFileDialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Загружаем изображение
                var image = new BitmapImage(new Uri(openFileDialog.FileName));

                // Устанавливаем изображение как фон для InkCanvas
                inkCanvas.Background = new ImageBrush(image);
            }
        }

        private void PenButton_Checked(object sender, RoutedEventArgs e)
        {
            StarSettings.Visibility = Visibility.Hidden;
            foreach (var canvas in InkCanvases)
            {
                canvas.EditingMode = InkCanvasEditingMode.Ink;
                canvas.MouseDown -= EllipseDrawing_MouseDown;
                canvas.MouseMove -= EllipseDrawing_MouseMove;
                canvas.MouseUp -= EllipseDrawing_MouseUp;
                canvas.MouseDown -= LineDrawing_MouseDown;
                canvas.MouseMove -= LineDrawing_MouseMove;
                canvas.MouseUp -= LineDrawing_MouseUp;
                canvas.MouseDown -= StarDrawing_MouseDown;
            }
        }

        private void EraserButton_Checked(object sender, RoutedEventArgs e)
        {
            StarSettings.Visibility = Visibility.Hidden;
            foreach (var canvas in InkCanvases)
            {
                canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                canvas.MouseDown -= EllipseDrawing_MouseDown;
                canvas.MouseMove -= EllipseDrawing_MouseMove;
                canvas.MouseUp -= EllipseDrawing_MouseUp;
                canvas.MouseDown -= LineDrawing_MouseDown;
                canvas.MouseMove -= LineDrawing_MouseMove;
                canvas.MouseUp -= LineDrawing_MouseUp;
                canvas.MouseDown -= StarDrawing_MouseDown;
            }
        }

        private void LineButton_Checked(object sender, RoutedEventArgs e)
        {
            StarSettings.Visibility = Visibility.Hidden;
            foreach (var canvas in InkCanvases)
            {
                // Удаляем обработчики событий для других инструментов
                canvas.MouseDown -= EllipseDrawing_MouseDown;
                canvas.MouseMove -= EllipseDrawing_MouseMove;
                canvas.MouseUp -= EllipseDrawing_MouseUp;
                canvas.MouseDown -= StarDrawing_MouseDown;

                canvas.EditingMode = InkCanvasEditingMode.None;
                canvas.MouseDown += LineDrawing_MouseDown;
                canvas.MouseMove += LineDrawing_MouseMove;
                canvas.MouseUp += LineDrawing_MouseUp;
            }
        }
        private void EllipseButton_Checked(object sender, RoutedEventArgs e)
        {
            StarSettings.Visibility = Visibility.Hidden;
            foreach (var canvas in InkCanvases)
            {
                // Удаляем обработчики событий для других инструментов
                canvas.MouseDown -= LineDrawing_MouseDown;
                canvas.MouseMove -= LineDrawing_MouseMove;
                canvas.MouseUp -= LineDrawing_MouseUp;
                canvas.MouseDown -= StarDrawing_MouseDown;

                canvas.EditingMode = InkCanvasEditingMode.None;
                canvas.MouseDown += EllipseDrawing_MouseDown;
                canvas.MouseMove += EllipseDrawing_MouseMove;
                canvas.MouseUp += EllipseDrawing_MouseUp; 
            }
        }
        private void StarButton_Checked(object sender, RoutedEventArgs e)
        {
            StarSettings.Visibility = Visibility.Visible;
            foreach(var canvas in InkCanvases)
            {
                canvas.MouseDown -= LineDrawing_MouseDown;
                canvas.MouseMove -= LineDrawing_MouseMove;
                canvas.MouseUp -= LineDrawing_MouseUp;
                canvas.MouseDown -= EllipseDrawing_MouseDown;
                canvas.MouseMove -= EllipseDrawing_MouseMove;
                canvas.MouseUp -= EllipseDrawing_MouseUp;

                canvas.EditingMode = InkCanvasEditingMode.None;
                canvas.MouseDown += StarDrawing_MouseDown;
            }
        }

        #region Линия
        private void LineDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var inkCanvas = sender as InkCanvas;
            if (inkCanvas == null) return;

            currentLine = new Line
            {
                Stroke = new SolidColorBrush(PenColor),
                StrokeThickness = PenSize,
                X1 = e.GetPosition(inkCanvas).X,
                Y1 = e.GetPosition(inkCanvas).Y,
                X2 = e.GetPosition(inkCanvas).X,
                Y2 = e.GetPosition(inkCanvas).Y
            };

            inkCanvas.Children.Add(currentLine);
        }
        private void LineDrawing_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentLine != null)
            {
                var inkCanvas = sender as InkCanvas;
                if (inkCanvas == null) return;

                currentLine.X2 = e.GetPosition(inkCanvas).X;
                currentLine.Y2 = e.GetPosition(inkCanvas).Y;
            }
        }
        private void LineDrawing_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentLine = null;
        }
        #endregion

        #region Эллипс
        private void EllipseDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var inkCanvas = sender as InkCanvas;
            if (inkCanvas == null) return;

            // Запоминаем начальную точку
            currentPoint = e.GetPosition(inkCanvas);

            // Создаем эллипс
            currentEllipse = new Ellipse
            {
                Stroke = new SolidColorBrush(PenColor),
                StrokeThickness = PenSize,
            };

            inkCanvas.Children.Add(currentEllipse);
        }

        private void EllipseDrawing_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentEllipse != null)
            {
                var inkCanvas = sender as InkCanvas;
                if (inkCanvas == null) return;

                // Удаляем предыдущий эллипс
                if (inkCanvas.Children.Contains(currentEllipse))
                {
                    inkCanvas.Children.Remove(currentEllipse);
                }

                // Создаем новый эллипс
                currentEllipse = new Ellipse
                {
                    Stroke = new SolidColorBrush(PenColor),
                    StrokeThickness = PenSize,
                    Width = Math.Abs(e.GetPosition(inkCanvas).X - currentPoint.X),
                    Height = Math.Abs(e.GetPosition(inkCanvas).Y - currentPoint.Y)
                };

                InkCanvas.SetLeft(currentEllipse, Math.Min(e.GetPosition(inkCanvas).X, currentPoint.X));
                InkCanvas.SetTop(currentEllipse, Math.Min(e.GetPosition(inkCanvas).Y, currentPoint.Y));

                inkCanvas.Children.Add(currentEllipse);
            }
        }

        private void EllipseDrawing_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentEllipse = null;
        }

        #endregion

        #region Звезда
        /// <summary>
        /// Событие клика при рисовании звезды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StarDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var inkCanvas = sender as InkCanvas;
            if (inkCanvas == null) return;

            // Получаем координаты клика мыши
            var mousePosition = e.GetPosition(inkCanvas);

            // Получаем параметры звезды из текстовых полей
            int numPoints = int.Parse(StarPointCount.Text);
            double innerRadiusRatio = double.Parse(RadiusQuotient.Text);

            // Создаем звезду
            var star = CreateStar(mousePosition, numPoints, innerRadiusRatio);

            // Добавляем звезду на холст
            //var starStroke = new Stroke(new StylusPointCollection(((Polygon)star).Points.Select(p => new StylusPoint(p.X, p.Y))));
            inkCanvas.Children.Add(star);
        }
        /// <summary>
        /// Логика рисования звезды
        /// </summary>
        /// <param name="center">Точка нажатия</param>
        /// <param name="numPoints">Число лучей</param>
        /// <param name="innerRadiusRatio">Отношение радиусов</param>
        /// <returns></returns>
        private Shape CreateStar(Point center, int numPoints, double innerRadiusRatio)
        {
            var points = new PointCollection();
            double angleStep = Math.PI / numPoints;
            double outerRadius = 50; // Вы можете изменить это значение в соответствии с вашими потребностями
            double innerRadius = outerRadius * innerRadiusRatio;

            for (int i = 0; i < 2 * numPoints; i++)
            {
                double radius = i % 2 == 0 ? outerRadius : innerRadius;
                double angle = i * angleStep;
                points.Add(new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle)));
            }

            var star = new Polygon
            {
                Points = points,
                Stroke = new SolidColorBrush(PenColor), // Используйте вашу глобальную переменную PenColor
                StrokeThickness = PenSize, // Используйте вашу глобальную переменную PenSize
                Fill = Brushes.Transparent
            };
            return star;
        }
        /// <summary>
        /// Запрет на ввод нечисел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StarPointCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Запрет на ввод нечисел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiusQuotient_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Автоустановка 0 в поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StarPointCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
            }
        }
        /// <summary>
        /// Автоустановка 0 в поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadiusQuotient_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
            }
        }
        #endregion

        #region Масштаб
        /// <summary>
        /// Увеличение масштаба активного холста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            var activeMdiChild = MdiContainer.ActiveMdiChild as MdiChild;
            if (activeMdiChild == null) return;
            var activeInkCanvas = activeMdiChild.Content as InkCanvas;
            ScaleTransform scale = (ScaleTransform)activeInkCanvas.LayoutTransform;
            scale.ScaleX *= 2;
            scale.ScaleY *= 2;
        }
        /// <summary>
        /// Уменьшение масштаба активного холста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            var activeMdiChild = MdiContainer.ActiveMdiChild as MdiChild;
            if (activeMdiChild == null) return;
            var activeInkCanvas = activeMdiChild.Content as InkCanvas;
            ScaleTransform scale = (ScaleTransform)activeInkCanvas.LayoutTransform;
            scale.ScaleX /= 2;
            scale.ScaleY /= 2;
        }
        #endregion
    }
}
