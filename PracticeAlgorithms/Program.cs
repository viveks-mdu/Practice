using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PracticeAlgorithms
{
    class Program
    {

        public enum EnvironmentType
        {
            Unknown = 0,
            PROD = 1,
            RD = 2,
            eDog = 3,
            spDF = 4,
            ProdBubble = 5,
            BlackForest = 6,
            Gallatin = 7,
            Gov = 8,
            Trailblazer = 9,
            Pathfinder = 10,
            ag08 = 11,
            ag09 = 12,
            ProdDebug = 13,
            BuildLab = 14
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Program started ...");

            /*
            EnvironmentType containerEnvironment = EnvironmentType.RD;
            string environmentValue = "spdf";
            Enum.TryParse<EnvironmentType>(environmentValue, true, out containerEnvironment);
            */

            /*
            //var a = new List<int> { 3, 4, -1, -2, 6, 9, -4, 2 };
            var a = new List<int> { 3};
            var res = FindMaxSubArray(a, 0, a.Count - 1);
            Console.WriteLine($"min: {res.Item1}; max: {res.Item2}; maxSum: {res.Item3}");
            //var res = FindMaxCrossingSubArray(a, 0, 3, 7);
            */

            /*
            TestTaskDelayAsync().Wait();
            */

            /*
            WriteStreamAsync().Wait();
            */

            Console.WriteLine("Program completed.");
        }

        public static Tuple<int, int, int> FindMaxSubArray(List<int> a, int low, int high)
        {
            if (low > high)
            {
                return Tuple.Create(-1, -1, 0);
            }
            else if (low == high)
            {
                return Tuple.Create(low, high, a[low]);
            }
            else
            {
                int mid = (low + high) / 2; //auto floor

                var leftMax = FindMaxSubArray(a, low, mid);
                var rightMax = FindMaxSubArray(a, mid + 1, high);
                var crossMax = FindMaxCrossingSubArray(a, low, mid, high);

                var res = leftMax.Item3 >= rightMax.Item3 ?
                    (leftMax.Item3 >= crossMax.Item3 ? leftMax : crossMax) : (rightMax.Item3 >= crossMax.Item3 ? rightMax : crossMax);

                Console.WriteLine($"low: {low}; mid: {mid}; high: {high}; res: {res}");

                return res;
            }
        }

        public static Tuple<int, int, int> FindMaxCrossingSubArray(List<int> a, int low, int mid, int high)
        {
            if (low > mid || mid >= high)
            {
                return Tuple.Create(-1, -1, int.MinValue);
            }

            int leftSum = a[mid];
            int leftMaxSum = leftSum;
            int leftIndex = mid;

            for (int i = mid - 1; i >= low; i--)
            {
                leftSum += a[i];

                if (leftSum > leftMaxSum)
                {
                    leftMaxSum = leftSum;
                    leftIndex = i;
                }
            }

            int rightSum = a[mid + 1];
            int rightMaxSum = rightSum;
            int rightIndex = mid + 1;

            for (int j = mid + 2; j <= high; j++)
            {
                rightSum += a[j];

                if (rightSum > rightMaxSum)
                {
                    rightMaxSum = rightSum;
                    rightIndex = j;
                }
            }

            var crossRes = Tuple.Create(leftIndex, rightIndex, leftMaxSum + rightMaxSum);

            Console.WriteLine($"\tCross result - low: {low}; mid: {mid}; high: {high}; crossRes: {crossRes}");

            return crossRes;
        }

        public static async Task TestTaskDelayAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            Console.WriteLine("Print this await");
        }

        public const string TestFolder = "c:\\TestWorkItems\\Output";

        public static async Task WriteStreamAsync()
        {
            using (var fst = new FileStream(Path.Combine(TestFolder, "file1.jpg"), FileMode.Create, FileAccess.Write))
            {
                Stream st = new FileStream("c:\\TestWorkItems\\ilay.jpg", FileMode.Open, FileAccess.Read);

                await st.CopyToAsync(fst);
            }
        }
    }
}
