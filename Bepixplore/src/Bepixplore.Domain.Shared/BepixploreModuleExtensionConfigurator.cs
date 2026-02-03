using Bepixplore.Users;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Bepixplore;

public static class BepixploreModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           AbpUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        /* You can configure extra properties for the
         * entities defined in the modules used by your application.
         *
         * This class can be used to define these extra properties
         * with a high level, easy to use API.
         *
         * Example: Add a new property to the user entity of the identity module

           ObjectExtensionManager.Instance.Modules()
              .ConfigureIdentity(identity =>
              {
                  identity.ConfigureUser(user =>
                  {
                      user.AddOrUpdateProperty<string>( //property type: string
                          "SocialSecurityNumber", //property name
                          property =>
                          {
                              //validation rules
                              property.Attributes.Add(new RequiredAttribute());
                              property.Attributes.Add(new StringLengthAttribute(64) {MinimumLength = 4});

                              //...other configurations for this property
                          }
                      );
                  });
              });

         * See the documentation for more:
         * https://abp.io/docs/latest/framework/architecture/modularity/extending/module-entity-extensions
         */

        ObjectExtensionManager.Instance.Modules()
            .ConfigureIdentity(identity =>
            {
                identity.ConfigureUser(user =>
                {
                    // Profile Picture
                    user.AddOrUpdateProperty<string>(
                        "ProfilePictureUrl",
                        property =>
                        {
                            // Validation Rules
                            property.Attributes.Add(new StringLengthAttribute(500));

                            // UI Configurations
                            property.UI.OnEditForm.IsVisible = true;
                            property.UI.OnCreateForm.IsVisible = true;
                            property.UI.OnTable.IsVisible = true;

                        }
                    );

                    // Notification Channel
                    user.AddOrUpdateProperty<NotificationChannel>(
                        "NotificationChannel",
                        property =>
                        {
                            property.Attributes.Add(new RequiredAttribute());

                            property.DefaultValue = NotificationChannel.Email;
                            property.UI.OnEditForm.IsVisible = true;
                        }
                    );

                    // Notification Frequency
                    user.AddOrUpdateProperty<NotificationFrequency>(
                        "NotificationFrequency",
                        property =>
                        {
                            property.Attributes.Add(new RequiredAttribute());
                            property.DefaultValue = NotificationFrequency.Immediate;

                            property.UI.OnEditForm.IsVisible = true;
                            property.UI.OnCreateForm.IsVisible = true;
                        }
                    );
                });
            });
    }
}
