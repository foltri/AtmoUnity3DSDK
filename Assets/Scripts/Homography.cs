using System;
using System.IO;
using System.IO.IsolatedStorage;
using UnityEngine;

public class Homography
{
    private static float[] _h;
    private static float[] _inv_h;

    // folder where calibration parameters are saved
    private string homeDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    private string folderName = ".atmo";
    // name of the file in which calibration parameters are saved
    private string fileName = "homography.json";

    public Homography()
    {
        LoadHomography();
    }

    public Vector2 GetWorldPosition(Vector2 pos)
    {
        float s = _h[6] * pos.x + _h[7] * pos.y + _h[8];
        float x = (_h[0] * pos.x + _h[1] * pos.y + _h[2]) / s;
        float y = (_h[3] * pos.x + _h[4] * pos.y + _h[5]) / s;
        return new Vector2(x, y);
    }

    public void SetHomography(Vector2[] src, Vector2[] dest)
    {
        _h = FindHomography(src, dest);

        // save h to config file
        var config = new HomographyConfig();
        config.h = _h;

        string json = JsonUtility.ToJson(config);

        // save json to file
        SaveNewHomography(json);
    }

    private string FixPath(string path)
    {
        string[] parts = path.Split('/');
        string correctPath = parts[0] + '/' + parts[1] + '/' + parts[2];
        return correctPath;
    }

    private void LoadHomography()
    {
        string json = null;

        homeDirectoryPath = FixPath(homeDirectoryPath);
        
        Debug.Log(homeDirectoryPath + '/' + folderName + '/' + fileName);

        try
        {
            json = File.ReadAllText(homeDirectoryPath + "/" + folderName + '/' + fileName);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Could not find folder '.atmo'. So there are no previous calibration parameters to use. " +
                             "You can start calibration by pressing 'c'.");

            // todo - could let user know that calibration is probably needed
        }
        // this means the same as the previous. don't know why it raises this instead sometimes.
        catch (IsolatedStorageException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Could not find folder '.atmo'. So there are no previous calibration parameters to use. " +
                             "You can start calibration by pressing 'c'.");

            // todo - could let user know that calibration is probably needed
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Could not find file '" + fileName + 
                             "'. So there are no previous calibration parameters to use. You can start calibration by pressing 'c'.");

            //todo - could let user know that calibration is probably needed
        }

        // could find parameters but empty
        if (json == "")
        {
            Debug.LogWarning("The file homography.json is empty. So there are no previous calibration parameters to use. " +
                             "You can start calibration by pressing 'c'.");
        }
        // could find parameter and not empty
        else if (json != null)
        {
            var conf = JsonUtility.FromJson<HomographyConfig>(json);
            _h = conf.h;
            
            Debug.Log("Previously saved calibration parameters are found and going to be used.");
        }
        // Best hint parameters. Better then nothing. 
        else
        {
            _h = FindHomography(
                new[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
                },
                new[]
                {
                    new Vector2(132, 0),
                    new Vector2(1280, 0),
                    new Vector2(172, 800),
                    new Vector2(1280, 800)
                }
            );
            
            Debug.Log("Dummy calibration parameters are used for now.");
        }
    }

    private void SaveNewHomography(string json)
    {
        if (!Directory.Exists(homeDirectoryPath))
        {
            Directory.CreateDirectory(homeDirectoryPath);
            Debug.Log("The directory .atmo was not found but it's created now.");
        }
        
        try
        {
            using (var fs = new FileStream(homeDirectoryPath + "/" + folderName + '/' + fileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }
            
            Debug.Log("New calibration parameters are saved in " + homeDirectoryPath + "/" + folderName + '/' + fileName);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Calibration parameters could not be saved. Could not find folder '.atmo', even though this function should have created it.");

        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            Debug.LogWarning("Calibration parameters could not be saved. Could not find file 'Homography.json', even though this function should have created it.");
        }
    }

    private float[] FindHomography(Vector2[] src, Vector2[] dest)
    {
        // originally by arturo castro - 08/01/2010  
        //  
        // create the equation system to be solved  
        //  
        // from: Multiple View Geometry in Computer Vision 2ed  
        //       Hartley R. and Zisserman A.  
        //  
        // x' = xH  
        // where H is the homography: a 3 by 3 matrix  
        // that transformed to inhomogeneous coordinates for each point  
        // gives the following equations for each point:  
        //  
        // x' * (h31*x + h32*y + h33) = h11*x + h12*y + h13  
        // y' * (h31*x + h32*y + h33) = h21*x + h22*y + h23  
        //  
        // as the homography is scale independent we can let h33 be 1 (indeed any of the terms)  
        // so for 4 points we have 8 equations for 8 terms to solve: h11 - h32  
        // after ordering the terms it gives the following matrix  
        // that can be solved with gaussian elimination:  

        float[,] P =
        {
            {-src[0].x, -src[0].y, -1, 0, 0, 0, src[0].x * dest[0].x, src[0].y * dest[0].x, -dest[0].x}, // h11  
            {0, 0, 0, -src[0].x, -src[0].y, -1, src[0].x * dest[0].y, src[0].y * dest[0].y, -dest[0].y}, // h12  

            {-src[1].x, -src[1].y, -1, 0, 0, 0, src[1].x * dest[1].x, src[1].y * dest[1].x, -dest[1].x}, // h13  
            {0, 0, 0, -src[1].x, -src[1].y, -1, src[1].x * dest[1].y, src[1].y * dest[1].y, -dest[1].y}, // h21  

            {-src[2].x, -src[2].y, -1, 0, 0, 0, src[2].x * dest[2].x, src[2].y * dest[2].x, -dest[2].x}, // h22  
            {0, 0, 0, -src[2].x, -src[2].y, -1, src[2].x * dest[2].y, src[2].y * dest[2].y, -dest[2].y}, // h23  

            {-src[3].x, -src[3].y, -1, 0, 0, 0, src[3].x * dest[3].x, src[3].y * dest[3].x, -dest[3].x}, // h31  
            {0, 0, 0, -src[3].x, -src[3].y, -1, src[3].x * dest[3].y, src[3].y * dest[3].y, -dest[3].y} // h32  
        };

        P = GaussianElimination(P, 9);

        float h11 = P[0, 8];
        float h12 = P[1, 8];
        float h13 = P[2, 8];
        float h21 = P[3, 8];
        float h22 = P[4, 8];
        float h31 = P[6, 8];
        float h32 = P[7, 8];
        float h23 = P[5, 8];
        float h33 = 1f;

        return new[] {h11, h12, h13, h21, h22, h23, h31, h32, h33};
    }

    private float[,] GaussianElimination(float[,] A, int n)
    {
        // originally by arturo castro - 08/01/2010  
        //  
        // ported to c from pseudocode in  
        // http://en.wikipedia.org/wiki/Gaussian_elimination  

        int i = 0;
        int j = 0;
        int m = n - 1;
        while (i < m && j < n)
        {
            // Find pivot in column j, starting in row i:  
            int maxi = i;
            for (int k = i + 1; k < m; k++)
                if (Mathf.Abs(A[k, j]) > Mathf.Abs(A[maxi, j]))
                    maxi = k;
            if (A[maxi, j] != 0)
            {
                //swap rows i and maxi, but do not change the value of i  
                if (i != maxi)
                    for (int k = 0; k < n; k++)
                    {
                        float aux = A[i, k];
                        A[i, k] = A[maxi, k];
                        A[maxi, k] = aux;
                    }

                //Now A[i,j] will contain the old value of A[maxi,j].  
                //divide each entry in row i by A[i,j]  
                float A_ij = A[i, j];
                for (int k = 0; k < n; k++) A[i, k] /= A_ij;
                //Now A[i,j] will have the value 1.  
                for (int u = i + 1; u < m; u++)
                {
                    //subtract A[u,j] * row i from row u  
                    float A_uj = A[u, j];
                    for (int k = 0; k < n; k++) A[u, k] -= A_uj * A[i, k];
                    //Now A[u,j] will be 0, since A[u,j] - A[i,j] * A[u,j] = A[u,j] - 1 * A[u,j] = 0.  
                }

                i++;
            }

            j++;
        }

        //back substitution  
        for (int k = m - 2; k >= 0; k--)
        for (int l = k + 1; l < n - 1; l++)
            A[k, m] -= A[k, l] * A[l, m];
        //A[i*n+j]=0;  

        return A;
    }
}