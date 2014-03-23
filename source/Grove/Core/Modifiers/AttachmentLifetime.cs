﻿namespace Grove.Modifiers
{
  using System;
  using Grove.Events;
  using Grove.Infrastructure;

  public class AttachmentLifetime : Lifetime, IReceive<AttachmentDetached>
  {
    private readonly Func<Lifetime, Card> _selector;

    private AttachmentLifetime() {}

    public AttachmentLifetime(Func<Lifetime, Card> selector = null)
    {
      _selector = selector ?? (self => self.Modifier.SourceCard);
    }

    public void Receive(AttachmentDetached message)
    {
      var attachment = _selector(this);

      if (message.Attachment == attachment)
      {
        End();
      }
    }
  }
}