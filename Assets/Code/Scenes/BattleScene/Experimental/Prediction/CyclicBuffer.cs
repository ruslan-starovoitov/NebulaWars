namespace Code.Prediction
{
    public class CyclicBuffer
    {
        //todo сделать циклический буффер на 1 сек
        //todo менять размер буффера по дисперсии 
        public class CyclicBufferElement
        {
            public object userInput;
            public object predictedUserTransform;
        }
    }
}