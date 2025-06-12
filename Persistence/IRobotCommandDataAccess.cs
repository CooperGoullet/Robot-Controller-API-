
namespace robot_controller_api.Persistence
{
    public interface IRobotCommandDataAccess
    {
        List<RobotCommand> GetRobotCommands();
        RobotCommand UpdateRobotCommand(int id, RobotCommand command);
        void DeleteRobotCommand(int id);
        void InsertRobotCommand(RobotCommand command);
    }

}