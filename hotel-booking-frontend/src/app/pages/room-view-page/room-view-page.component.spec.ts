import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomViewPageComponent } from './room-view-page.component';

describe('RoomViewPageComponent', () => {
  let component: RoomViewPageComponent;
  let fixture: ComponentFixture<RoomViewPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomViewPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomViewPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
