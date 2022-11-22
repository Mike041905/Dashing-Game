using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class NotificationManager : MonoBehaviour
{
    static NotificationManager _instance;
    public static NotificationManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }


    const int CollectOfflineCoinsID = 0;
    const string DefaultNotificationChanelID = "default";


    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        AndroidNotificationChannel channel = new()
        {
            Id = DefaultNotificationChanelID,
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification(string title, string text, DateTime time)
    {
        AndroidNotification notification = new()
        {
            Title = title,
            Text = text,
            FireTime = time
        };

        AndroidNotificationCenter.SendNotification(notification, DefaultNotificationChanelID);
    }
    public void SendNotification(string title, string text, DateTime time, int id)
    {
        AndroidNotification notification = new()
        {
            Title = title,
            Text = text,
            FireTime = time
        };

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, DefaultNotificationChanelID, id);
    }

    public void SendCollectCoinsNotification()
    {
        NotificationStatus coinsNotificationSatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(CollectOfflineCoinsID);

        if(coinsNotificationSatus == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.UpdateScheduledNotification
            (
                CollectOfflineCoinsID,
                new
                (
                    "Collect your coins",
                    "Hey! I seams like you've reached the max amount of coins offline!",
                    DateTime.Now.AddHours(10)
                ),
                DefaultNotificationChanelID
            );
        }
        else if (coinsNotificationSatus == NotificationStatus.Delivered)
        {
            AndroidNotificationCenter.CancelNotification(CollectOfflineCoinsID);
        }
        else
        {
            SendNotification
            (
                "Collect your coins",
                "Hey! I seams like you've reached the max amount of coins offline!",
                DateTime.Now.AddHours(10),
                CollectOfflineCoinsID
            );
        }
    }
}
