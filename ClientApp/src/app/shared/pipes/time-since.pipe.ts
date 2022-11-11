import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
  name: 'timeSince'
})
export class TimeSincePipe implements PipeTransform {

  transform(value: string): string {
    return this.timeSince(value);
  }

  timeSince(date: Date | string) {
    const formatDate = typeof date === 'string' ? new Date(date) : date;
    const currentDate = new Date();

    let seconds = Math.floor((currentDate.getTime() - formatDate.getTime()) / 1000);

    let interval = seconds / 31536000;
    if (interval > 1) {
      if (Math.floor(interval) === 1) {
        return `${Math.floor(interval)} year ago`;
      } else {
        return `${Math.floor(interval)} years ago`;
      }
    }

    interval = seconds / 2592000;
    if (interval > 1) {
      if (Math.floor(interval) === 1) {
        return `${Math.floor(interval)} month ago`;
      } else {
        return `${Math.floor(interval)} months ago`;
      }
    }

    interval = seconds / 86400;
    if (interval > 1) {
      if (Math.floor(interval) === 1) {
        return `${Math.floor(interval)} day ago`;
      } else {
        return `${Math.floor(interval)} days ago`;
      }
    }

    interval = seconds / 3600;
    if (interval > 1) {
      if (Math.floor(interval) === 1) {
        return `${Math.floor(interval)} hour ago`;
      } else {
        return `${Math.floor(interval)} hours ago`;
      }
    }

    interval = seconds / 60;
    if (interval > 1) {
      if (Math.floor(interval) === 1) {
        return `${Math.floor(interval)} minute ago`;
      } else {
        return `${Math.floor(interval)} minutes ago`;
      }
    }

    return 'just now';
  }
}
