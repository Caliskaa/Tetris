using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private const int Rows = 20;
        private const int Columns = 10;
        private int[,] gameField = new int[Rows, Columns];
        private Figure currentFigure;
        private DispatcherTimer timer;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                StartGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
            }
        }

        private void StartGame()
        {
            ClearField();
            SpawnNewFigure();
            SetupTimer();
            this.KeyDown += MainWindow_KeyDown; // Подписка на событие клавиатуры
            DrawGameField();
        }

        private void ClearField()
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    gameField[row, col] = 0;
        }

        private void SpawnNewFigure()
        {
            currentFigure = Figure.CreateRandomFigure();

            // Убедитесь, что фигура не выходит за границы
            if (!CanMoveDown(currentFigure))
            {
                timer.Stop();
                MessageBox.Show("Игра окончена!");
                StartGame(); // Перезапуск игры
            }
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveFigureDown();
            DrawGameField();
        }

        private void MoveFigureDown()
        {
            if (CanMoveDown(currentFigure))
            {
                currentFigure.MoveDown();
            }
            else
            {
                PlaceFigure();
                CheckForCompleteLines();
                SpawnNewFigure();
            }
        }

        // Проверка возможности движения фигуры вниз
        private bool CanMoveDown(Figure figure)
        {
            for (int i = 0; i < figure.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < figure.Shape.GetLength(1); j++)
                {
                    if (figure.Shape[i, j] != 0) // Проверяем, есть ли часть фигуры
                    {
                        int newX = figure.PositionX + j; // Текущий X
                        int newY = figure.PositionY + i + 1; // Новый Y (на одну строку ниже)

                        // Проверка границ массива
                        if (newY >= Rows) // Если новая Y выходит за пределы
                        {
                            return false; // Нельзя двигаться вниз
                        }

                        if (newX < 0 || newX >= Columns) // Если новая X выходит за пределы
                        {
                            return false; // Нельзя двигаться в сторону
                        }

                        // Проверка на столкновение с другими фигурами
                        if (newY >= 0 && newY < Rows && newX >= 0 && newX < Columns)
                        {
                            if (gameField[newY, newX] != 0)
                            {
                                return false; // Если сталкивается с другой фигурой
                            }
                        }
                    }
                }
            }
            return true; // Если все проверки пройдены, движение вниз возможно
        }

        // Размещение фигуры на игровом поле
        private void PlaceFigure()
        {
            for (int i = 0; i < currentFigure.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < currentFigure.Shape.GetLength(1); j++)
                {
                    if (currentFigure.Shape[i, j] != 0) // Проверяем, есть ли часть фигуры
                    {
                        int posX = currentFigure.PositionX + j;
                        int posY = currentFigure.PositionY + i;

                        // Проверка границ перед записью в массив
                        if (posX >= 0 && posX < Columns && posY >= 0 && posY < Rows)
                            gameField[posY, posX] = 1; // Или другой идентификатор цвета/фигуры
                    }
                }
            }
        }

        // Проверка и удаление заполненных линий
        private void CheckForCompleteLines()
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                bool isComplete = true;
                for (int col = 0; col < Columns; col++)
                {
                    if (gameField[row, col] == 0)
                    {
                        isComplete = false;
                        break;
                    }
                }
                if (isComplete)
                {
                    // Удаление линии и сдвиг всех линий выше вниз
                    for (int r = row; r > 0; r--)
                        for (int c = 0; c < Columns; c++)
                            gameField[r, c] = gameField[r - 1, c];

                    // Обнуление верхней линии
                    for (int c = 0; c < Columns; c++)
                        gameField[0, c] = 0;

                    row++; // Проверить эту строку снова
                }
            }
        }

        // Обработка нажатий клавиш
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MoveFigureLeft();
                    break;
                case Key.Right:
                    MoveFigureRight();
                    break;
                case Key.Down:
                    MoveFigureDown(); // Ускорение падения
                    break;
                case Key.Up:
                    RotateFigure();
                    break;
                case Key.Space:
                    DropFigure(); // Мгновенное падение
                    break;
            }
            DrawGameField(); // Обновляем отображение после нажатия клавиши
        }

        private void MoveFigureLeft()
        {
            currentFigure.MoveLeft();
            if (!CanMoveDown(currentFigure))
                currentFigure.MoveRight(); // Возврат назад если движение невозможно
        }

        private void MoveFigureRight()
        {
            currentFigure.MoveRight();
            if (!CanMoveDown(currentFigure))
                currentFigure.MoveLeft(); // Возврат назад если движение невозможно
        }

        private void RotateFigure()
        {
            currentFigure.Rotate();
            if (!CanMoveDown(currentFigure))
                currentFigure.Rotate(); // Возврат назад если вращение невозможно
        }

        private void DropFigure()
        {
            while (CanMoveDown(currentFigure))
            {
                currentFigure.MoveDown();
            }
            PlaceFigure();
            CheckForCompleteLines();
            DrawGameField();

            if (!CanMoveDown(currentFigure))
            {
                timer.Stop();
                MessageBox.Show("Игра окончена!");
                StartGame();
            }
        }

        private void DrawGameField()
        {
            GameCanvas.Children.Clear();

            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                {
                    if (gameField[row, col] != 0)
                    {
                        Rectangle rect = new Rectangle { Width = GameCanvas.ActualWidth / Columns, Height = GameCanvas.ActualHeight / Rows, Fill = Brushes.White };
                        Canvas.SetLeft(rect, col * rect.Width);
                        Canvas.SetTop(rect, row * rect.Height);
                        GameCanvas.Children.Add(rect);
                    }
                }

            DrawCurrentFigure();
        }

        private void DrawCurrentFigure()
        {
            for (int i = 0; i < currentFigure.Shape.GetLength(0); i++)
                for (int j = 0; j < currentFigure.Shape.GetLength(1); j++)
                {
                    if (currentFigure.Shape[i, j] != 0)
                    {
                        Rectangle rect = new Rectangle { Width = GameCanvas.ActualWidth / Columns, Height = GameCanvas.ActualHeight / Rows, Fill = Brushes.Red };
                        Canvas.SetLeft(rect, (currentFigure.PositionX + j) * rect.Width);
                        Canvas.SetTop(rect, (currentFigure.PositionY + i) * rect.Height);
                        GameCanvas.Children.Add(rect);
                    }
                }
        }
    }
}