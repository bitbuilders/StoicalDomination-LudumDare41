﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Globalization;

public class Timer : Singleton<Timer>
{
    [SerializeField] Text m_timeText = null;
    [SerializeField] bool m_playOnAwake = true;

    StringBuilder m_stringBuilder;
    AudioSource m_audioSource;
    float m_time;
    int m_startingFont;
    int m_blinkFont;
    bool m_playing = false;
    bool m_hasBlinked = false;
    public bool Finished { get; set; }
    public bool Paused { get { return !m_playing; } }

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_stringBuilder = new StringBuilder();
        ResetTimer(10.0f);
        Finished = false;
        m_startingFont = m_timeText.fontSize;
        m_blinkFont = m_startingFont / 2;
        if (m_playOnAwake) Resume();
    }

    private void Update()
    {
        if (m_playing)
        {
            if (m_time > 0.0f)
            {
                m_time -= Time.deltaTime;
            }
            else
            {
                ResetTimer(10.0f);
            }

            if (m_time <= 6.0f && !m_hasBlinked)
            {
                m_hasBlinked = true;
                StartCoroutine(BlinkText());
            }

            UpdateTimer();
        }
    }

    public void ResetTimer(float timeForNextPlayer)
    {
        m_time = timeForNextPlayer;
        UpdateTimer();
        m_hasBlinked = false;
        Finished = true;
        m_playing = false;
    }

    private void UpdateTimer()
    {
        m_stringBuilder.Remove(0, m_stringBuilder.Length);
        int minutes = (int)m_time / 60;
        int seconds = (int)m_time % 60;
        int milliseconds = (int)((m_time * 1000) % 1000);
        m_stringBuilder.Append(minutes.ToString("D2"));
        m_stringBuilder.Append("::");
        m_stringBuilder.Append(seconds.ToString("D2"));
        m_stringBuilder.Append("::");
        m_stringBuilder.Append(milliseconds.ToString("D3"));

        m_timeText.text = m_stringBuilder.ToString();
    }

    public void Pause()
    {
        m_playing = false;
    }

    public void Resume()
    {
        m_playing = true;
        Finished = false;
    }

    IEnumerator BlinkText()
    {
        StartCoroutine(Blink());
        m_audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Blink());
        m_audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Blink());
        m_audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Blink());
        m_audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Blink());
        m_audioSource.Play();
    }

    IEnumerator Blink()
    {
        for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 5.0f)
        {
            int fontSize = m_startingFont + (int)(i * m_blinkFont);
            m_timeText.fontSize = fontSize;
            yield return null;
        }

        for (float i = 1.0f; i >= 0.0f; i -= Time.deltaTime * 5.0f)
        {
            int fontSize = m_startingFont + (int)(i * m_blinkFont);
            m_timeText.fontSize = fontSize;
            yield return null;
        }

        m_timeText.fontSize = m_startingFont;
    }
}
