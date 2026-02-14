// UMD IMDM290 
// Instructor: Myungin Lee
// [a <-----------> b]
// Lerp : Linearly interpolates between two points. 
// https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Vector3.Lerp.html

using UnityEngine;

public class Lerp1 : MonoBehaviour
{
    GameObject[] spheres;
    static int numSphere = 200; 
    float time = 3f * Mathf.PI / 2f, prevLerp = 0f;
    int offset = 0;
    bool nadir = true;
    Vector3[] startPosition, endPositions;

    // Start is called before the first frame update
    void Start()
    {
        // Assign proper types and sizes to the variables.
        spheres = new GameObject[numSphere];
        startPosition = new Vector3[numSphere];
        endPositions = new Vector3[4 * numSphere];
        
        // Define target positions. Start = random, End = heart 
        for (int i =0; i < numSphere; i++){
            // Random start positions
            float r = 10f;
            startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));        
            
            float t = i / (float)numSphere;
            float cos = Mathf.Cos(2 * Mathf.PI * t);
            float sin = Mathf.Sin(2 * Mathf.PI * t);
            r = 3f;

            //L
            if (i < numSphere / 3f) {
                endPositions[i] = new Vector3(r * (4 * t - (2f / 3f)), -2f);
            } 
            else {
                endPositions[i] = new Vector3(r * -(2f / 3f), r * (3 * t - (5f / 3f)));
            }

            // U
            if (i < numSphere / 3f)
            {
                endPositions[i + numSphere] = new Vector3(-2f, r * 4 * t);
            } else if (i < 2f * numSphere / 3f) {
                endPositions[i + numSphere] = new Vector3(-2f * Mathf.Cos(3 * Mathf.PI * (t - (1f / 3f))), -2f * Mathf.Sin(3 * Mathf.PI * (t - (1f / 3f))));
            } else {
                endPositions[i + numSphere] = new Vector3(2f, r * (4 * t - (8f / 3f)));
            }

            // V
            if (i < numSphere / 2f) {
                endPositions[i + numSphere * 2] = new Vector3(r * (4f / 3f * t - (2f / 3f)), r * (-4f * t + (4f / 3f)));
            } else {
                endPositions[i + numSphere * 2] = new Vector3(r * (4f / 3f * t - (2f / 3f)), r * (4f * t - (8f / 3f)));
            }


            // Circular end position
            // endPosition[i] = new Vector3(r * Mathf.Sin(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere));
            endPositions[i + numSphere * 3] = new Vector3(
                r * Mathf.Sqrt(2) * Mathf.Pow(sin, 3), 
                r * (2 * cos - Mathf.Pow(cos, 3) - Mathf.Pow(cos, 2)) + 2
            );
        }
        // Let there be spheres..
        for (int i =0; i < numSphere; i++){
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere); 

            // Position
            spheres[i].transform.position = startPosition[i];

            // Color. Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Measure Time 
        time += Time.deltaTime; // Time.deltaTime = The interval in seconds from the last frame to the current one
        // what to update over time?

        // lerpFraction variable defines the point between startPosition and endPosition (0~1)
        // let it oscillate over time using sin function
        float lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

        if (!nadir && lerpFraction - prevLerp > 0) { //when lerpFraction starts to increase (toward end position)
            nadir = true;
            offset = offset + numSphere >= numSphere * 4 ? 0 : offset + numSphere;
        } else if (nadir && lerpFraction - prevLerp < 0) { //when lerpFraction starts to decrease (toward start position)
            nadir = false;
        }
        prevLerp = lerpFraction;

        for (int i =0; i < numSphere; i++){
            // Lerp : Linearly interpolates between two points.
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
            // Vector3.Lerp(startPosition, endPosition, lerpFraction)

            // Lerp logic. Update position
            spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPositions[i + offset], lerpFraction);
            // For now, start positions and end positions are fixed. But what if you change it over time?
            // startPosition[i]; endPosition[i];

            // Color Update over time
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            float hue = (float)i / numSphere; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Sin(time)), Mathf.Cos(time), 2f + Mathf.Cos(time)); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }
}
