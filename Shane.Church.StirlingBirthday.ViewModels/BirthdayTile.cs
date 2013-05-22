using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shane.Church.Utility;
using NLog;

namespace Shane.Church.StirlingBirthday.ViewModels
{
	public static class BirthdayTile
	{
		public static string Path = "/Shared/ShellContent/BirthdayFront.png";
		public static string BackPath = "/Shared/ShellContent/BirthdayBack.png";
		public static string ScheduledTaskName = "StirlingBirthdayUpdater";
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static void UpdateTile(IEnumerable<ContactViewModel> model, TaskCompletionSource<bool> tcs, bool useContactUri = false)
		{
			BirthdayTile.BuildTileImage(model.FirstOrDefault(), useContactUri).ContinueWith(bti =>
			{
				BirthdayTile.BuildBackTileImage(model).ContinueWith(bbti =>
				{
					BirthdayTile.UpdateTileData().ContinueWith(utd =>
					{
						tcs.TrySetResult(true);
					});
				});
			});
		}


		private static Task BuildTileImage(ContactViewModel model, bool useUri = false)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
			_logger.Trace("In BuildTileImage");
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					BitmapImage overlayBitmap = new BitmapImage(new Uri(@"/Images/BirthdayOverlay.png", UriKind.Relative));
					BitmapImage standardIconBitmap = new BitmapImage(new Uri(@"/Background.png", UriKind.Relative));
					WriteableBitmap wbm = new WriteableBitmap(173, 173);

					var canvas = new Canvas()
					{
						Height = 173,
						Width = 173
					};

					if ((!useUri && (model != null && model.Picture != null && model.Picture.PixelHeight > 0 && model.Picture.PixelWidth > 0)) || (useUri && model.PictureUri != null))
					{
						_logger.Trace("Contact has picture");
						Image overlayImage = new Image();
						overlayImage.Width = 173;
						overlayImage.Height = 173;
						var overlayImageTask = EventAsync.FromEvent<RoutedEventArgs>(overlayImage, "ImageOpened").ContinueWith(oit =>
						{
							Image contactImage = new Image();
							contactImage.Width = 173;
							contactImage.Height = 173;
							var contactImageTask = EventAsync.FromEvent<RoutedEventArgs>(contactImage, "ImageOpened").ContinueWith(t =>
							{
								Deployment.Current.Dispatcher.BeginInvoke(() =>
								{
									try
									{
										Canvas.SetLeft(contactImage, 0);
										Canvas.SetTop(contactImage, 0);
										canvas.Children.Add(contactImage);
									}
									catch (Exception ex)
									{
										_logger.DebugException("Canvas Contact Image", ex);
										tcs.TrySetException(ex);
									}
									try
									{
										Canvas.SetLeft(overlayImage, 0);
										Canvas.SetTop(overlayImage, 0);
										canvas.Children.Add(overlayImage);
									}
									catch (Exception ex)
									{
										_logger.DebugException("Canvas Overlay Image", ex);
										tcs.TrySetException(ex);
									}

									try
									{
										wbm.Render(canvas, null);
										wbm.Invalidate();
									}
									catch (Exception ex)
									{
										_logger.DebugException("Front tile wbm render", ex);
										tcs.TrySetException(ex);
									}

									try
									{
										CompensateForRender(wbm.Pixels);
									}
									catch (Exception ex)
									{
										_logger.DebugException("Front tile compensate for render", ex);
										tcs.TrySetException(ex);
									}

									try
									{
										Imaging.SaveImage(wbm, BirthdayTile.Path);
									}
									catch (Exception ex)
									{
										_logger.DebugException("Front tile save image", ex);
										tcs.TrySetException(ex);
									}

									tcs.TrySetResult(true);
								});
							});
							try
							{
								if (useUri)
								{
									contactImage.Source = new BitmapImage(model.PictureUri);
								}
								else
								{
									contactImage.Source = model.Picture;
								}
							}
							catch (Exception ex)
							{
								_logger.DebugException("Front Tile Contact Image", ex);
								tcs.TrySetException(ex);
							}
						});
						try
						{
							overlayImage.Source = overlayBitmap;
						}
						catch (Exception ex)
						{
							_logger.DebugException("Front Tile Overlay Image", ex);
							tcs.TrySetException(ex);
						}
					}
					else
					{
						_logger.Trace("Contact doesn't have picture");
						Image standardIconImage = new Image();
						standardIconImage.Width = 173;
						standardIconImage.Height = 173;
						var standardIconTask = EventAsync.FromEvent<RoutedEventArgs>(standardIconImage, "ImageOpened").ContinueWith(sit =>
						{
							Deployment.Current.Dispatcher.BeginInvoke(() =>
							{
								try
								{
									Canvas.SetLeft(standardIconImage, 0);
									Canvas.SetTop(standardIconImage, 0);
									canvas.Children.Add(standardIconImage);
								}
								catch (Exception ex)
								{
									_logger.DebugException("Canvas Standard Image", ex);
									tcs.TrySetException(ex);
								}

								try
								{
									wbm.Render(canvas, null);
									wbm.Invalidate();
								}
								catch (Exception ex)
								{
									_logger.DebugException("Front tile wbm render", ex);
									tcs.TrySetException(ex);
								}

								try
								{
									CompensateForRender(wbm.Pixels);
								}
								catch (Exception ex)
								{
									_logger.DebugException("Front tile compensate for render", ex);
									tcs.TrySetException(ex);
								}

								try
								{
									Imaging.SaveImage(wbm, BirthdayTile.Path);
								}
								catch (Exception ex)
								{
									_logger.DebugException("Front tile save image", ex);
									tcs.TrySetException(ex);
								}

								tcs.TrySetResult(true);
							});
						});
						try
						{
							standardIconImage.Source = standardIconBitmap;
						}
						catch (Exception ex)
						{
							_logger.DebugException("Front Tile Standard Image", ex);
							tcs.TrySetException(ex);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.ErrorException("Front tile unknown error", ex);
					tcs.TrySetException(ex);
				}
			});
			return tcs.Task;
		}

		private static Task BuildBackTileImage(IEnumerable<ContactViewModel> model)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					WriteableBitmap wbm = new WriteableBitmap(173, 173);

					for (int i = 0; i < model.Count() && i < 3; i++)
					{
						TextBlock tbName = new TextBlock();
						tbName.Text = model.ElementAt(i).DisplayName;
						tbName.FontSize = 20;
						tbName.FontFamily = (FontFamily)Application.Current.Resources["PhoneFontFamilySemiBold"];
						tbName.HorizontalAlignment = HorizontalAlignment.Left;
						tbName.Foreground = new SolidColorBrush(Colors.White);
						tbName.Width = 173;
						tbName.Padding = new Thickness(5, 4, 5, 4);
						wbm.Render(tbName, new TranslateTransform()
						{
							Y = (28 * i) + (16 * i)
						});
						wbm.Invalidate();
						TextBlock tbDate = new TextBlock();
						tbDate.Text = model.ElementAt(i).StartTileDateText;
						tbDate.FontSize = 16;
						tbDate.FontFamily = (FontFamily)Application.Current.Resources["PhoneFontFamilyNormal"];
						tbDate.Padding = new Thickness(5, 0, 5, 0);
						tbDate.Foreground = new SolidColorBrush(Colors.White);
						tbDate.HorizontalAlignment = HorizontalAlignment.Right;
						tbDate.Width = 173;
						wbm.Render(tbDate, new TranslateTransform()
						{
							Y = (28 * (i + 1)) + (16 * i)
						});
						wbm.Invalidate();
					}

					CompensateForRender(wbm.Pixels);

					Imaging.SaveImage(wbm, BirthdayTile.BackPath);

					tcs.TrySetResult(true);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			});
			return tcs.Task;
		}

		private static Task UpdateTileData()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				ShellTile mainTile = ShellTile.ActiveTiles.First();

				if (mainTile != null)
				{
					StandardTileData data = new StandardTileData()
					{
						BackgroundImage = new Uri("isostore:" + BirthdayTile.Path, UriKind.Absolute),
						Title = "Stirling Birthday",
						Count = 0,
						BackTitle = "Stirling Birthday",
						BackBackgroundImage = new Uri("isostore:" + BirthdayTile.BackPath, UriKind.Absolute)
					};
					mainTile.Update(data);
				}

				tcs.TrySetResult(true);
			});
			return tcs.Task;
		}

		private static void CompensateForRender(int[] bitmapPixels)
		{
			if (bitmapPixels.Length == 0) return;

			for (int i = 0; i < bitmapPixels.Length; i++)
			{
				uint pixel = unchecked((uint)bitmapPixels[i]);

				double a = (pixel >> 24) & 255;
				if ((a == 255) || (a == 0)) continue;

				double r = (pixel >> 16) & 255;
				double g = (pixel >> 8) & 255;
				double b = (pixel) & 255;

				double factor = 255 / a;
				uint newR = (uint)Math.Round(r * factor);
				uint newG = (uint)Math.Round(g * factor);
				uint newB = (uint)Math.Round(b * factor);

				// compose
				bitmapPixels[i] = unchecked((int)((pixel & 0xFF000000) | (newR << 16) | (newG << 8) | newB));
			}
		}
	}
}
