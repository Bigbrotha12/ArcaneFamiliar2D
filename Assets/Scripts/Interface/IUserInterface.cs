using System.Collections.Generic;
using Characters;

namespace UserInterface
{
  public interface IUserInterface 
  {
    public void QueueAlert(string message, float delay);
    public void QueueAlert(string message, string[] options, Response playerReply);
    public void QueueObject(IObjectHeader artifact, float delay);
    public void QueueObject(IObjectHeader artifact, string[] options, ObjectResponse playerReply);
    public void QueueConversation(CharacterSO speaker, List<string> speech, float textSpeed);
    public void QueueConversation(CharacterSO speaker, List<string> speech, string[] options, Response playerReply, float textSpeed);
    }
}