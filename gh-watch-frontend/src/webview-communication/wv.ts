/* eslint-disable @typescript-eslint/no-explicit-any */
declare global {
  interface Window {
    chrome: {
      webview: {
        postMessage: (data: unknown) => void;
        addEventListener: (
          event: string,
          handler: (event: unknown) => void,
        ) => void;
        removeEventListener: (
          event: string,
          handler: (event: unknown) => void,
        ) => void;
      };
    };
  }
}

export interface ReceiveMessageProps {
  id?: string;
  eventType: string;
  payload: object;
}

type MessageHandler<T = any> = (payload: T, id?: string) => Promise<any> | any;

interface MessageHandlerMap {
  [eventType: string]: MessageHandler;
}

export const registerWebViewMessageHandlers = (handlers: MessageHandlerMap) => {
  const messageListener = async (event: any) => {
    if (window.chrome?.webview === undefined) {
      return;
    }
    try {
      const data = event.data as ReceiveMessageProps & { id?: string };

      if (data && data.eventType && handlers[data.eventType]) {
        const result = await handlers[data.eventType](data.payload, data.id);

        // if handler returns something, echo it back
        if (data.id && result !== undefined) {
          postMessage({
            id: data.id,
            command: "RESPONSE",
            payload: { value: result },
          });
        }
      }
    } catch (ex) {
      console.error("Error handling WebView message:", ex);
    }
  };

  window.chrome.webview.addEventListener("message", messageListener);

  return () => {
    window.chrome.webview.removeEventListener("message", messageListener);
  };
};
