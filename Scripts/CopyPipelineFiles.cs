// Decompiled with JetBrains decompiler
// Type: Microsoft.Web.Publishing.Tasks.CopyPipelineFiles
// Assembly: Microsoft.Web.Publishing.Tasks, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0DFE4B21-3D4F-4288-8EDE-6F3E5EDBE982
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\Web\Microsoft.Web.Publishing.Tasks.dll

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Web.Publishing.Tasks.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.Web.Publishing.Tasks
{
  public sealed class CopyPipelineFiles : Task
  {
    private bool m_skipMetadataExcludeTrueItems = true;
    private bool m_deleteItemsMarkAsExcludeTrue = true;
    private int m_maxRetries = 3;
    private int m_retryDelay = 300;
    private ITaskItem[] m_pipeLineItemsITaskItem;
    private List<ITaskItem> m_updateItemsITaskItem;
    private bool m_updateItemSpec;

    [Required]
    public ITaskItem[] PipelineItems
    {
      get
      {
        return this.m_pipeLineItemsITaskItem;
      }
      set
      {
        this.m_pipeLineItemsITaskItem = value;
      }
    }

    [Output]
    public ITaskItem[] ResultPipelineItems
    {
      get
      {
        return this.m_pipeLineItemsITaskItem;
      }
    }

    [Output]
    public ITaskItem[] UpdatedPipelineItems
    {
      get
      {
        if (this.m_updateItemsITaskItem != null)
          return this.m_updateItemsITaskItem.ToArray();
        return (ITaskItem[]) null;
      }
    }

    [Required]
    public string TargetDirectory { get; set; }

    [Required]
    public string SourceDirectory { get; set; }

    public bool SkipMetadataExcludeTrueItems
    {
      get
      {
        return this.m_skipMetadataExcludeTrueItems;
      }
      set
      {
        this.m_skipMetadataExcludeTrueItems = value;
      }
    }

    public bool DeleteItemsMarkAsExcludeTrue
    {
      get
      {
        return this.m_deleteItemsMarkAsExcludeTrue;
      }
      set
      {
        this.m_deleteItemsMarkAsExcludeTrue = value;
      }
    }

    public int MaxRetries
    {
      get
      {
        return this.m_maxRetries;
      }
      set
      {
        this.m_maxRetries = value;
      }
    }

    public int RetryDelay
    {
      get
      {
        return this.m_retryDelay;
      }
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException("value", SR.BUILDTASK_CopyFilesToFolders_RetryDelayOutOfRange);
        this.m_retryDelay = value;
      }
    }

    public bool UpdateItemSpec
    {
      get
      {
        return this.m_updateItemSpec;
      }
      set
      {
        this.m_updateItemSpec = value;
      }
    }

    public override bool Execute()
    {
      ItemFilter.ItemMetadataFilter itemMetadataSkipFilter = (ItemFilter.ItemMetadataFilter) null;
      if (this.SkipMetadataExcludeTrueItems)
        itemMetadataSkipFilter = new ItemFilter.ItemMetadataFilter(ItemFilter.ItemFilterExcludeTrue);
      List<ITaskItem> failedPipeLineItems = new List<ITaskItem>(8);
      this.m_updateItemsITaskItem = new List<ITaskItem>(this.m_pipeLineItemsITaskItem == null ? 0 : this.m_pipeLineItemsITaskItem.GetLength(0));
      bool folder = CopyPipelineFiles.CopyPipelineFilesToFolder(this.Log, this.m_pipeLineItemsITaskItem, this.SourceDirectory, this.TargetDirectory, itemMetadataSkipFilter, this.UpdateItemSpec, this.DeleteItemsMarkAsExcludeTrue, this.m_updateItemsITaskItem, failedPipeLineItems);
      if (folder && failedPipeLineItems.Count > 0)
      {
        for (int index = 1; index < this.MaxRetries & folder && failedPipeLineItems != null; ++index)
        {
          Thread.Sleep(this.RetryDelay * (index > 0 ? index : 1));
          ITaskItem[] array = failedPipeLineItems.ToArray();
          failedPipeLineItems.Clear();
          if (this.MaxRetries == index + 1)
            failedPipeLineItems = (List<ITaskItem>) null;
          folder = CopyPipelineFiles.CopyPipelineFilesToFolder(this.Log, array, this.SourceDirectory, this.TargetDirectory, itemMetadataSkipFilter, this.UpdateItemSpec, this.DeleteItemsMarkAsExcludeTrue, this.m_updateItemsITaskItem, failedPipeLineItems);
        }
      }
      return folder;
    }

    internal static bool CopyPipelineFilesToFolder(TaskLoggingHelper log, ITaskItem[] allpipeLineItems, string sourceFolderName, string targetFolderName, ItemFilter.ItemMetadataFilter itemMetadataSkipFilter, bool fUpdateItemSpec, bool deleteItemsMarkAsExcludeTrue, List<ITaskItem> updatedPipeLineItems, List<ITaskItem> failedPipeLineItems)
    {
      bool flag1 = failedPipeLineItems == null;
      string currentDirectory = Directory.GetCurrentDirectory();
      if (!string.IsNullOrEmpty(sourceFolderName) && string.Compare(Path.GetFullPath(sourceFolderName), currentDirectory, StringComparison.OrdinalIgnoreCase) == 0)
        sourceFolderName = (string) null;
      if (!string.IsNullOrEmpty(sourceFolderName) && !Path.IsPathRooted(targetFolderName))
        targetFolderName = Path.Combine(currentDirectory, targetFolderName);
      bool flag2 = CopyPipelineFiles.CreateDirectoryInNeeded(log, targetFolderName);
      if (flag2)
      {
        foreach (ITaskItem allpipeLineItem in allpipeLineItems)
        {
          string str1 = Path.Combine(targetFolderName, allpipeLineItem.GetMetadata(PipelineMetadata.DestinationRelativePath.ToString()));
          if (itemMetadataSkipFilter != null && itemMetadataSkipFilter(allpipeLineItem))
          {
            if (deleteItemsMarkAsExcludeTrue)
            {
              if (File.Exists(str1))
              {
                try
                {
                  log.LogMessage(MessageImportance.Normal, string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.BUILDTASK_CopyFilesToFolders_Deleting, (object) str1), Array.Empty<object>());
                  File.Delete(str1);
                }
                catch (Exception ex)
                {
                  if (flag1 && ex is IOException)
                  {
                    failedPipeLineItems.Add(allpipeLineItem);
                  }
                  else
                  {
                    log.LogError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.BUILDTASK_CopyFilesToFolders_DeleteFailed, (object) str1, (object) ex.Message), Array.Empty<object>());
                    flag2 = false;
                    break;
                  }
                }
              }
            }
          }
          else
          {
            string str2 = allpipeLineItem.ItemSpec;
            if (!Path.IsPathRooted(str2) && !string.IsNullOrEmpty(sourceFolderName))
              str2 = Path.Combine(sourceFolderName, str2);
            try
            {
              bool flag3 = false;
              if (!File.Exists(str1))
              {
                flag3 = true;
              }
              else
              {
                FileInfo fileInfo1 = new FileInfo(str2);
                FileInfo fileInfo2 = new FileInfo(str1);
                if (fileInfo1.LastWriteTimeUtc > fileInfo2.LastWriteTimeUtc || fileInfo1.Length != fileInfo2.Length)
                  flag3 = true;
              }
              if (flag3)
              {
                log.LogMessage(MessageImportance.Normal, string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.BUILDTASK_CopyFilesToFolders_Copying, (object) str2, (object) str1), Array.Empty<object>());
                string directoryName = Path.GetDirectoryName(str1);
                flag2 = CopyPipelineFiles.CreateDirectoryInNeeded(log, directoryName);
                if (flag2)
                {
                  File.Copy(str2, str1, true);
                  File.SetAttributes(str1, FileAttributes.Normal);
                  updatedPipeLineItems.Add(allpipeLineItem);
                }
                else
                  break;
              }
              else
                log.LogMessage(MessageImportance.Normal, string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.BUILDTASK_CopyFilesToFolders_UpToDate, (object) str2, (object) str1), Array.Empty<object>());
              if (fUpdateItemSpec)
                allpipeLineItem.ItemSpec = str1;
            }
            catch (Exception ex)
            {
              if (flag1 && ex is IOException)
              {
                failedPipeLineItems.Add(allpipeLineItem);
              }
              else
              {
                log.LogError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.BUILDTASK_CopyFilesToFolders_CopyFailed, (object) str2, (object) str1, (object) ex.Message), Array.Empty<object>());
                flag2 = false;
                break;
              }
            }
          }
        }
      }
      return flag2;
    }

    internal static bool CreateDirectoryInNeeded(TaskLoggingHelper log, string folderName)
    {
      bool flag = true;
      if (!Directory.Exists(folderName))
      {
        try
        {
          Directory.CreateDirectory(folderName);
        }
        catch (Exception ex)
        {
          log.LogMessage(MessageImportance.Normal, SR.BUILDTASK_CreateFolder_Failed, (object) folderName, (object) ex.Message);
          flag = false;
        }
      }
      return flag;
    }
  }
}
