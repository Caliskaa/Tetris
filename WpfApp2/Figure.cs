using System;

namespace Tetris
{
    public class Figure
    {
        public int[,] Shape { get; private set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        // Размеры игрового поля
        private const int Columns = 10; // Количество колонок
        private const int Rows = 20;     // Количество строк

        // Объявляем массив фигур
        private static readonly int[][,] Shapes = new int[][,]
        {
            new int[,] { { 1, 1, 1, 1 } }, // I
            new int[,] { { 1, 1, 1 }, { 0, 1, 0 } }, // T
            new int[,] { { 1, 1 }, { 1, 1 } }, // O
            new int[,] { { 0, 1, 1 }, { 1, 1, 0 } }, // S
            new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }, // Z
            new int[,] { { 1, 0, 0 }, { 1, 1, 1 } }, // L
            new int[,] { { 0, 0, 1 }, { 1, 1, 1 } } // J
        };

        public Figure(int[,] shape)
        {
            Shape = shape;
            PositionX = Columns / 2 - shape.GetLength(1) / 2;
            PositionY = -shape.GetLength(0); // Начальная позиция выше поля для появления фигуры.
        }

        public static Figure CreateRandomFigure()
        {
            Random random = new Random();
            int index = random.Next(Shapes.Length);
            return new Figure(Shapes[index]);
        }

        public void MoveDown() => PositionY++;

        public void MoveLeft() => PositionX--;

        public void MoveRight() => PositionX++;

        public void Rotate()
        {
            int rows = Shape.GetLength(0);
            int cols = Shape.GetLength(1);
            int[,] rotatedShape = new int[cols, rows];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    rotatedShape[j, (rows - i) - 1] = Shape[i, j];

            Shape = rotatedShape;
        }
    }
}