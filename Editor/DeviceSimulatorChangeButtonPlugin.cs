using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    [UsedImplicitly]
    internal sealed class DeviceSimulatorChangeButtonPlugin : DeviceSimulatorPlugin
    {
        private static readonly Type         DEVICE_SIMULATOR_TYPE       = typeof( DeviceSimulator );
        private static readonly Type         SIMULATOR_WINDOW_TYPE       = DEVICE_SIMULATOR_TYPE.Assembly.GetType( "UnityEditor.DeviceSimulation.SimulatorWindow" );
        private static readonly Type         DEVICE_SIMULATOR_MAIN_TYPE  = DEVICE_SIMULATOR_TYPE.Assembly.GetType( "UnityEditor.DeviceSimulation.DeviceSimulatorMain" );
        private static readonly PropertyInfo DEVICES_PROPERTY            = DEVICE_SIMULATOR_MAIN_TYPE.GetProperty( "devices", BindingFlags.Instance | BindingFlags.Public );
        private static readonly PropertyInfo DEVICE_INDEX_PROPERTY       = DEVICE_SIMULATOR_MAIN_TYPE.GetProperty( "deviceIndex", BindingFlags.Instance | BindingFlags.Public );
        private static readonly FieldInfo    DEVICE_SIMULATOR_MAIN_FIELD = SIMULATOR_WINDOW_TYPE.GetField( "m_Main", BindingFlags.Instance | BindingFlags.NonPublic );

        public override string title => "Change Device";

        public override VisualElement OnCreateUI()
        {
            var prevButton = new Button( () => ChangeDevice( -1 ) )
            {
                text  = "Prev",
                style = { flexGrow = 1 },
            };

            var randomButton = new Button( () => RandomDevice() )
            {
                text  = "Random",
                style = { flexGrow = 1 },
            };

            var nextButton = new Button( () => ChangeDevice( 1 ) )
            {
                text  = "Next",
                style = { flexGrow = 1 },
            };

            var root = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row },
            };

            root.Add( prevButton );
            root.Add( randomButton );
            root.Add( nextButton );

            return root;
        }

        private static void ChangeDevice( int offset )
        {
            var simulatorWindow = Resources
                    .FindObjectsOfTypeAll( SIMULATOR_WINDOW_TYPE )
                    .FirstOrDefault()
                ;

            var deviceSimulatorMain = DEVICE_SIMULATOR_MAIN_FIELD.GetValue( simulatorWindow );
            var devices             = ( Array )DEVICES_PROPERTY.GetValue( deviceSimulatorMain );
            var deviceIndex         = ( int )DEVICE_INDEX_PROPERTY.GetValue( deviceSimulatorMain );
            var deviceCount         = devices.Length;
            var newDeviceIndex      = ( deviceIndex + offset + deviceCount ) % deviceCount;

            DEVICE_INDEX_PROPERTY.SetValue( deviceSimulatorMain, newDeviceIndex );
        }

        private static void RandomDevice()
        {
            var simulatorWindow = Resources
                    .FindObjectsOfTypeAll( SIMULATOR_WINDOW_TYPE )
                    .FirstOrDefault()
                ;

            var deviceSimulatorMain = DEVICE_SIMULATOR_MAIN_FIELD.GetValue( simulatorWindow );
            var devices             = ( Array )DEVICES_PROPERTY.GetValue( deviceSimulatorMain );
            var deviceIndex         = ( int )DEVICE_INDEX_PROPERTY.GetValue( deviceSimulatorMain );
            var deviceCount         = devices.Length;

            int newDeviceIndex;

            do
            {
                newDeviceIndex = UnityEngine.Random.Range( 0, deviceCount );
            } while ( newDeviceIndex == deviceIndex );

            DEVICE_INDEX_PROPERTY.SetValue( deviceSimulatorMain, newDeviceIndex );
        }
    }
}