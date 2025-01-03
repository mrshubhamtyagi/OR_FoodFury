using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool isPaused;

    public Coroutine StartCountdown(int _time, float _gapBetweenUpdateCallback, Action<float> _onUpdate = null, Action _onComplete = null)
    {
        return StartCoroutine(Co_StartCoundown(_time, _gapBetweenUpdateCallback, _onUpdate, _onComplete));
    }
    private IEnumerator Co_StartCoundown(int _time, float _gapBetweenUpdateCallback, Action<float> _onUpdate, Action _onComplete)
    {
        WaitForSeconds wait = new WaitForSeconds(_gapBetweenUpdateCallback);

        float counter = _time;
        _onUpdate?.Invoke(counter);
        yield return null;

        while (counter > 1)
        {
            //print(counter);
            yield return wait;
            if (!isPaused)
            {
                counter -= _gapBetweenUpdateCallback;
                _onUpdate?.Invoke(counter);
            }
        }

        yield return wait;
        _onUpdate?.Invoke(0);
        _onComplete?.Invoke();
    }


    public Coroutine StartTimer(int _startTime, int _endTime, float _gapBetweenUpdateCallback, Action<float> _onUpdate = null, Action _onComplete = null)
    {
        return StartCoroutine(Co_StartTimer(_startTime, _endTime, _gapBetweenUpdateCallback, _onUpdate, _onComplete));
    }
    private IEnumerator Co_StartTimer(int _startTime, int _endTime, float _gapBetweenUpdateCallback, Action<float> _onUpdate, Action _onComplete)
    {
        WaitForSeconds wait = new WaitForSeconds(_gapBetweenUpdateCallback);

        float counter = _startTime;
        _onUpdate?.Invoke(counter);
        yield return null;

        while (counter < _endTime)
        {
            //print(counter);
            yield return wait;
            counter += _gapBetweenUpdateCallback;
            _onUpdate?.Invoke(counter);
        }

        yield return wait;
        _onUpdate?.Invoke(_endTime);
        _onComplete?.Invoke();
    }


    public void StopTimer(Coroutine _timer) => StopCoroutine(_timer);

    public static string GetTimeInMinAndSec(float _time)
    {
        float minutes = Mathf.FloorToInt(_time / 60);
        float seconds = Mathf.FloorToInt(_time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static int GetMinutes(float _time) => Mathf.FloorToInt(_time / 60);

    public static int GetSeconds(float _time) => Mathf.FloorToInt(_time % 60);

    public static float GetTimeInSeconds(Tuple<string, string, string> _time) => float.Parse(_time.Item3) + float.Parse(_time.Item2) * 60;
}
