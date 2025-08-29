using System;
using System.Linq;
using Utils.Result;

namespace Core.GridSystem.Runtime
{
    public class GridTextParser
    {
        public Result<GridData> Parse(string textContent)
        {
            if (string.IsNullOrEmpty(textContent))
                return Result<GridData>.Fail();

            var linesResult = SplitIntoLines(textContent);
            if (!linesResult.IsExist)
                return Result<GridData>.Fail();

            var lines = linesResult.Object;
            if (!ValidateLines(lines))
                return Result<GridData>.Fail();

            var width = lines[0].Length;
            var height = lines.Length;
            var flatDataResult = ConvertToFlatArray(lines, width, height);

            if (!flatDataResult.IsExist)
                return Result<GridData>.Fail();

            var gridData = new GridData(flatDataResult.Object, width, height);
            return Result<GridData>.Success(gridData);
        }

        private Result<string[]> SplitIntoLines(string textContent)
        {
            try
            {
                var lines = textContent
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Trim())
                    .ToArray();

                return lines.Length > 0 ? Result<string[]>.Success(lines) : Result<string[]>.Fail();
            }
            catch
            {
                return Result<string[]>.Fail();
            }
        }

        private bool ValidateLines(string[] lines)
        {
            if (lines.Length == 0)
                return false;

            var expectedWidth = lines[0].Length;
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length != expectedWidth)
                    return false;
            }

            return true;
        }

        private Result<int[]> ConvertToFlatArray(string[] lines, int width, int height)
        {
            try
            {
                var result = new int[width * height];
                var index = 0;

                foreach (var line in lines)
                {
                    foreach (var character in line)
                    {
                        if (!char.IsDigit(character))
                            return Result<int[]>.Fail();

                        result[index] = character - '0';
                        index++;
                    }
                }

                return Result<int[]>.Success(result);
            }
            catch
            {
                return Result<int[]>.Fail();
            }
        }
    }
}