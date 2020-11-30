
using UnityEngine;
using System.Threading;
using System;

public class GyroController : MonoBehaviour 
{

    [SerializeField]
    public float alpha = 0.1f;
    private bool gyroEnabled; 
    private UnityEngine.Gyroscope gyro;
    private GameObject GyroControl;
    private Quaternion rot;

	private Quaternion initAttitude;
	private Quaternion initRot;

    // Smooth slerp factor
    private const float lowPassFilterFactor = 0.2f;

    // Kalman Filter variables
        int sleep_time = 100;

        // Initialise matrices and variables
        float[,] C = new float[2, 4] { { 1, 0, 0, 0 }, { 0, 0, 1, 0 } };
        float[,] P = new float[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        float[,] Q = new float[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
        float[,] R = new float[2, 2] { { 1, 0 }, { 0, 1 } };
        float[,] identity4 = new float[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };

        float[,] state_estimate = new float[4, 1] { { 0 }, { 0 }, { 0 }, { 0 } };

        float phi_hat = 0.0f;
        float theta_hat = 0.0f;

        // Calculate accelerometer offsets
        int N = 100;
        float phi_offset = 0.0f;
        float theta_offset = 0.0f;

        // Measured sampling time
        float dt = 0.0f;
        float start_time;

        float phi_acc;
        float theta_acc;



    private void Start()
     {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
 
        gyroEnabled = EnableGyro();

        initKalmanFilter();
     }


     private bool EnableGyro()
     {
         if (SystemInfo.supportsGyroscope)
         {
            gyro = Input.gyro;
            gyro.enabled = true;

			initAttitude = gyro.attitude;
			initRot = transform.rotation;
            return true;
         }
         return false;
     }



     private void Update()
     {

		 transform.rotation = Quaternion.Slerp(transform.rotation,GyroToUnity(initRot *
             (Quaternion.Inverse(initAttitude) * (/*KalmanFilter(*/gyro.attitude/*)*/))),lowPassFilterFactor);
     }

	 private static Quaternion GyroToUnity(Quaternion q)
     {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
     }


    private void initKalmanFilter()
    {
        for (int i = 0; i < N; i++)
        {
            phi_offset += gyro.userAcceleration.x;
            theta_offset += gyro.userAcceleration.y;
            Thread.Sleep(sleep_time);
        }
        
        start_time = Time.time;

        phi_offset /= N;
        theta_offset /= N;
    }



    // Compensatory Filter

    /*private void getGyroBias()
    {
        float timeIter = Time.time;

        for(int i = 0; i < ITERATION_NUMBER; i++)
        {
            bx += Input.acceleration.x;
            by += Input.acceleration.y;
            bz += Input.acceleration.z;
        }
        bx /= ITERATION_NUMBER;
        by /= ITERATION_NUMBER;
        bz /= ITERATION_NUMBER;
    }*/

    public Quaternion KalmanFilter(Quaternion att)
    {

        // Get accelerometer measurements and remove offsets
        phi_acc = gyro.userAcceleration.x;
        theta_acc = gyro.userAcceleration.y;
        phi_acc -= phi_offset;
        theta_acc -= theta_offset;

        // Get gyro measurements and calculate Euler angle derivatives
        float p = att.eulerAngles.x;
        float q = att.eulerAngles.y;
        float r = att.eulerAngles.z;

        float phi_dot = p + Mathf.Sin(phi_hat) * Mathf.Tan(theta_hat) * q + Mathf.Cos(phi_hat) * Mathf.Tan(theta_hat) * r;
        float theta_dot = Mathf.Cos(phi_hat) * q - Mathf.Sin(phi_hat) * r;

        // Kalman filter
        float[,] A = new float[4, 4] { { 1, -dt, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, -dt }, { 0, 0, 0, 1 } };
        float[,] B = new float[4, 2] { { dt, 0 }, { 0, 0 }, { 0, dt }, { 0, 0 } };

        float[,] gyro_input = new float[2, 1] { { phi_dot }, { theta_dot } };
        if (gyro_input == null) Debug.Log("Es gyro_input");
        if (A == null) Debug.Log("Es A");
        if (B == null) Debug.Log("Es B");
        state_estimate = MatrixSum(MatrixProduct(A, state_estimate) , MatrixProduct(B, gyro_input));
        P = MatrixSum(MatrixProduct(A, MatrixProduct(P,Transpose(A))),Q);

        float[,] measurement = new float[2, 1] { { phi_acc }, { theta_acc } };
        float[,] y_tilde = MatrixSubstract(measurement , MatrixProduct(C,state_estimate));
        float[,] S = MatrixSum(R, MatrixProduct(C, MatrixProduct(P, Transpose(C))));
        float[,] K = MatrixProduct(P, (MatrixProduct(Transpose(C), (MatrixInverse(S)))));
        state_estimate = MatrixSum(state_estimate , MatrixProduct(K, (y_tilde)));
        P = MatrixProduct(MatrixSubstract(identity4 , MatrixProduct(K,C)), P);

        phi_hat = state_estimate[0,0];
        theta_hat = state_estimate[2,0];

            // Display results
            //    print("Phi: " + str(round(phi_hat * 180.0 / pi, 1)) + " Theta: " + str(round(theta_hat * 180.0 / pi, 1)))

        Thread.Sleep(sleep_time);

        return Quaternion.Euler(phi_hat, theta_hat, att.eulerAngles.z);
    }

    private float[] MatrixProduct(float[][] matrixA, float[] vectorB)
    {
        int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
        int bRows = vectorB.Length;
        if (aCols != bRows)
            throw new Exception("Non-conformable matrices in MatrixProduct");
        float[] result = new float[aRows];
        for (int i = 0; i < aRows; ++i) // each row of A
            for (int k = 0; k < aCols; ++k)
                result[i] += matrixA[i][k] * vectorB[k];
        return result;
    }
    public float[,] MatrixProduct(float[,] matrixA, float[,] matrixB)
    {
        if (matrixA == null) Debug.Log("Es A");
        if (matrixB == null) Debug.Log("Es B");
        Debug.Log($"{matrixB.Length}, {matrixA.GetLength(0)}, {matrixB.GetLength(1)}");
        int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
        int bRows = matrixB.GetLength(0); int bCols = matrixB.GetLength(1);
        if (aCols != bRows)
            throw new Exception("Non-conformable matrices in MatrixProduct");
        float[,] result = MatrixCreate(aRows, bCols);
        for (int i = 0; i < aRows; ++i) // each row of A
            for (int j = 0; j < bCols; ++j) // each col of B
                for (int k = 0; k < aCols; ++k)
                    result[i,j] += matrixA[i,k] * matrixB[k,j];
        return result;
    }

    public float[,] MatrixSum(float[,] matrixA, float[,] matrixB)
    {
        //Debug.Log($"{matrixA.Length}, {matrixA.GetLength(0)}, {matrixA.GetLength(1)}");
        int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
        int bRows = matrixB.GetLength(0); int bCols = matrixB.GetLength(1);
        if (aCols != bCols || aRows != bRows)
            throw new Exception("Non-conformable matrices in MatrixSum");
        float[,] result = MatrixCreate(aRows, bCols);
        for (int i = 0; i < aRows; ++i) // each row of A
            for (int j = 0; j < aCols; ++j) // each col of B
                    result[i, j] += matrixA[i, j] + matrixB[i, j];
        return result;
    }

    public float[,] MatrixSubstract(float[,] matrixA, float[,] matrixB)
    {
        Debug.Log($"{matrixA.Length}, {matrixA.GetLength(0)}, {matrixA.GetLength(1)}");
        int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
        int bRows = matrixB.GetLength(0); int bCols = matrixB.GetLength(1);
        if (aCols != bCols || aRows != bRows)
            throw new Exception("Non-conformable matrices in MatrixSum");
        float[,] result = MatrixCreate(aRows, bCols);
        for (int i = 0; i < aRows; ++i) // each row of A
            for (int j = 0; j < aCols; ++j) // each col of B
                result[i, j] += matrixA[i, j] - matrixB[i, j];
        return result;
    }

    public float[,] MatrixCreate(int rows, int cols)
    {
        // creates a matrix initialized to all 0.0s  
        // do error checking here?  
        float[,] result = new float[rows,cols];
        
        return result;
    }

    public float[,] Transpose(float[,] matrix)
    {
        int w = matrix.GetLength(0);
        int h = matrix.GetLength(1);

        float[,] result = new float[h, w];

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    public float[,] MatrixDuplicate(float[,] matrix)
    {
        // allocates/creates a duplicate of a matrix.
        float[,] result = MatrixCreate(matrix.Length, matrix.GetLength(0));
        for (int i = 0; i < matrix.GetLength(0); ++i) // copy the values
            for (int j = 0; j < matrix.GetLength(1); ++j)
                result[i,j] = matrix[i,j];
        return result;
    }

    public float[,] MatrixInverse(float[,] matrix)
    {
        int n = matrix.GetLength(0);
        float[,] result = MatrixDuplicate(matrix);

        int[] perm;
        int toggle;
        float[,] lum = MatrixDecompose(matrix, out perm, out toggle);
        if (lum == null)
            throw new Exception("Unable to compute inverse");

        float[] b = new float[n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (i == perm[j])
                    b[j] = 1.0f;
                else
                    b[j] = 0.0f;
            }

            float[] x = HelperSolve(lum, b);

            for (int j = 0; j < n; ++j)
                result[j,i] = x[j];
        }
        return result;
    }

    public float[] HelperSolve(float[,] luMatrix, float[] b)
    {
        // before calling this helper, permute b using the perm array
        // from MatrixDecompose that generated luMatrix
        int n = luMatrix.GetLength(0);
        float[] x = new float[n];
        b.CopyTo(x, 0);
        Debug.Log($"n: {n}");
        Debug.Log($"n2: {luMatrix.GetLength(1)}");

        for (int i = 1; i < n; ++i)
        {
            float sum = x[i];
            for (int j = 0; j < i; ++j)
            {
                Debug.Log($"i: {i}");
                Debug.Log($"j: {j}");
                sum -= luMatrix[i, j] * x[j];
            }
            x[i] = sum;
        }

        x[n - 1] /= luMatrix[n - 1,n - 1];
        for (int i = n - 2; i >= 0; --i)
        {
            float sum = x[i];
            for (int j = i + 1; j < n; ++j)
                sum -= luMatrix[i,j] * x[j];
            x[i] = sum / luMatrix[i,i];
        }

        return x;
    }

    public float[,] MatrixDecompose(float[,] matrix, out int[] perm, out int toggle)
    {
        // Doolittle LUP decomposition with partial pivoting.
        // rerturns: result is L (with 1s on diagonal) and U;
        // perm holds row permutations; toggle is +1 or -1 (even or odd)
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1); // assume square
        if (rows != cols)
            throw new Exception("Attempt to decompose a non-square m");

        int n = rows; // convenience

        float[,] result = MatrixDuplicate(matrix);

        perm = new int[n]; // set up row permutation result
        for (int i = 0; i < n; ++i) { perm[i] = i; }

        toggle = 1; // toggle tracks row swaps.
                    // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

        for (int j = 0; j < n - 1; ++j) // each column
        {
            float colMax = Math.Abs(result[j,j]); // find largest val in col
            int pRow = j;
            //for (int i = j + 1; i less-than n; ++i)
            //{
            //  if (result[i][j] greater-than colMax)
            //  {
            //    colMax = result[i][j];
            //    pRow = i;
            //  }
            //}

            // reader Matt V needed this:
            for (int i = j + 1; i < n; ++i)
            {
                if (Math.Abs(result[i,j]) > colMax)
                {
                    colMax = Math.Abs(result[i,j]);
                    pRow = i;
                }
            }
            // Not sure if this approach is needed always, or not.

            if (pRow != j) // if largest value not on pivot, swap rows
            {
                float[] rowPtr = copyRow(result, pRow, cols);
                for (int contador = 0; contador < cols; contador++)
                {
                    result[pRow, contador] = result[j, contador];
                }
                for (int contador = 0; contador < cols; contador++)
                {
                    result[j, contador] = rowPtr[contador];
                }

                int tmp = perm[pRow]; // and swap perm info
                perm[pRow] = perm[j];
                perm[j] = tmp;

                toggle = -toggle; // adjust the row-swap toggle
            }

            // --------------------------------------------------
            // This part added later (not in original)
            // and replaces the 'return null' below.
            // if there is a 0 on the diagonal, find a good row
            // from i = j+1 down that doesn't have
            // a 0 in column j, and swap that good row with row j
            // --------------------------------------------------

            if (result[j,j] == 0.0)
            {
                // find a good row to swap
                int goodRow = -1;
                for (int row = j + 1; row < n; ++row)
                {
                    if (result[row,j] != 0.0)
                        goodRow = row;
                }

                if (goodRow == -1)
                    throw new Exception("Cannot use Doolittle's method");

                // swap rows so 0.0 no longer on diagonal
                float[] rowPtr = copyRow(result,goodRow, cols);
                for(int contador = 0; contador < cols; contador++)
                {
                    result[goodRow, contador] = result[j, contador];
                }
                for (int contador = 0; contador < cols; contador++)
                {
                    result[j, contador] = rowPtr[contador];
                }

                int tmp = perm[goodRow]; // and swap perm info
                perm[goodRow] = perm[j];
                perm[j] = tmp;

                toggle = -toggle; // adjust the row-swap toggle
            }
            // --------------------------------------------------
            // if diagonal after swap is zero . .
            //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
            //  return null; // consider a throw

            for (int i = j + 1; i < n; ++i)
            {
                result[i,j] /= result[j,j];
                for (int k = j + 1; k < n; ++k)
                {
                    result[i,k] -= result[i,j] * result[j,k];
                }
            }


        } // main j column loop

        return result;
    }

    public float[] copyRow(float[,] matrix, int row, int lengthRow)
    {
        float[] result = new float [lengthRow];
        for(int i = 0; i<lengthRow; i++)
        {
            result[i] = matrix[row, i];
        }

        return result;
    }

    
}
