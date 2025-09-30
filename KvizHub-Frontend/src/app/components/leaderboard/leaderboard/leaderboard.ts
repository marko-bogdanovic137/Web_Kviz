import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeaderboardService } from '../../../services/leaderboard.service';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './leaderboard.html',
  styleUrl: './leaderboard.css'
})
export class LeaderboardComponent implements OnInit {
  leaderboard: any[] = [];
  timeFrame: string = 'all';
  isLoading: boolean = true;

  constructor(private leaderboardService: LeaderboardService) {}

  ngOnInit(): void {
    this.loadLeaderboard();
  }

  loadLeaderboard(): void {
    this.isLoading = true;
    this.leaderboardService.getGlobalLeaderboard(this.timeFrame).subscribe({
      next: (data) => {
        this.leaderboard = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading leaderboard:', error);
        this.loadMockData();
        this.isLoading = false;
      }
    });
  }

  onTimeFrameChange(): void {
    this.loadLeaderboard();
  }

  // Mock data za demo
  loadMockData(): void {
    this.leaderboard = [
      { rank: 1, username: 'super_igrac', totalScore: 450, quizzesCompleted: 15, averagePercentage: 92 },
      { rank: 2, username: 'kviz_majstor', totalScore: 420, quizzesCompleted: 12, averagePercentage: 88 },
      { rank: 3, username: 'pametnjakovic', totalScore: 380, quizzesCompleted: 10, averagePercentage: 85 },
      { rank: 4, username: 'admin', totalScore: 350, quizzesCompleted: 8, averagePercentage: 82 },
      { rank: 5, username: 'ucenik', totalScore: 320, quizzesCompleted: 7, averagePercentage: 78 },
      { rank: 6, username: 'pocetnik', totalScore: 280, quizzesCompleted: 6, averagePercentage: 75 },
      { rank: 7, username: 'igrac123', totalScore: 250, quizzesCompleted: 5, averagePercentage: 72 },
      { rank: 8, username: 'test_korisnik', totalScore: 220, quizzesCompleted: 4, averagePercentage: 68 },
      { rank: 9, username: 'novajlija', totalScore: 180, quizzesCompleted: 3, averagePercentage: 65 },
      { rank: 10, username: 'gost', totalScore: 150, quizzesCompleted: 2, averagePercentage: 60 }
    ];
  }

  getRankClass(rank: number): string {
    if (rank === 1) return 'rank-first';
    if (rank === 2) return 'rank-second';
    if (rank === 3) return 'rank-third';
    return 'rank-other';
  }
}