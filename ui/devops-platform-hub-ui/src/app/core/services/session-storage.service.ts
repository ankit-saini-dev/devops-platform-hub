import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SessionStorageService {
  public setItem(key: string, value: unknown): void {
    sessionStorage.setItem(key, JSON.stringify(value));
  }

  public getItem(key: string): unknown | null {
    const data = sessionStorage.getItem(key);
    if (!data) {
      return null;
    }

    try {
      return JSON.parse(data);
    } catch {
      return null;
    }
  }

  public removeItem(key: string): void {
    sessionStorage.removeItem(key);
  }
}
